using System;
using System.Collections.Generic;
using System.Linq;
using PointOfSale.Discounts;
using PointOfSale.SaleItems;

namespace PointOfSale
{
    public class PointOfSaleTerminalSession
    {
        private readonly IDiscountCardStorage m_discountCardStorage;
        private readonly ISaleItemDataProvider m_dataStore;
        private readonly Dictionary<string, SaleItemCheckEntry> m_check;
        private DiscountCard m_discountCard;
        private bool m_closed;

        public PointOfSaleTerminalSession(ISaleItemDataProvider dataStore, IDiscountCardStorage discountCardStorage)
        {
            m_discountCardStorage = discountCardStorage;
            m_dataStore = dataStore;
            m_check = new Dictionary<string, SaleItemCheckEntry>();
        }

        public void UseDiscountCard(Guid cardId)
        {
            m_discountCard = m_discountCardStorage.GetCardById(cardId);
        }

        public void Scan(string code)
        {
            if (m_check.TryGetValue(code, out SaleItemCheckEntry entry))
            {
                entry.Increment();

                return;
            }

            var item = m_dataStore.GetByCode(code);

            if (item != default) ScanNewItem(item);
        }

        private void ScanNewItem(SaleItem item)
        {
            if (item == default) throw new ArgumentNullException(nameof(item));
            if (item.Code == default) throw new ArgumentException("Invalid item code. Value cannot be null.");

            m_check.Add(item.Code, new SaleItemCheckEntry(item, 1));
        }

        public decimal CalculateTotal()
        {
            return m_check.Aggregate
            (
                new decimal(0),
                (total, entry) => total + entry.Value.CalculatePrice(m_discountCard)
            );
        }

        public void CloseSession()
        {
            if (!m_closed && m_discountCard != default)
            {
                m_discountCard.Charge(GetFullPriceTotal());
                m_discountCardStorage.Update(m_discountCard);
                m_discountCardStorage.Save();

                m_closed = true;
            }
        }

        private decimal GetFullPriceTotal()
        {
            return m_check.Aggregate
            (
                new decimal(0),
                (total, entry) => total + entry.Value.CalculateFullPrice()
            );
        }

        class SaleItemCheckEntry
        {
            public SaleItemCheckEntry(SaleItem item, int count)
            {
                if (count < 0) throw new ArgumentException("Count cannot be less then 0.", nameof(count));

                Item = item;
                Count = count;
            }

            public SaleItem Item { get; }
            public int Count { get; private set; }

            public void Increment()
            {
                Count++;
            }

            public decimal CalculatePrice(IDiscountProvider discountProvider = default)
            {
                if (Count == 0) return 0;

                if (Item.HasVolumePrice)
                {
                    var volumesCount = Count / Item.VolumePrice.Count;
                    var volumePrice = CalculateVolumePrice();
                    var singleItemsCount = Count - volumesCount * Item.VolumePrice.Count;
                    var singleItemsPrice = singleItemsCount * Item.Price;

                    return volumePrice + ApplyDiscount(singleItemsPrice, discountProvider);
                }

                return ApplyDiscount(CalculateFullPrice(), discountProvider);
            }

            public decimal CalculateFullPrice()
            {
                return Count * Item.Price;
            }

            private decimal ApplyDiscount(decimal price, IDiscountProvider discountProvider)
            {
                return discountProvider != default
                    ? discountProvider.ApplyDiscount(price)
                    : price;
            }

            private decimal CalculateVolumePrice()
            {
                if (!Item.HasVolumePrice) return 0;

                return Count / Item.VolumePrice.Count * Item.VolumePrice.Price;
            }
        }
    }
}
