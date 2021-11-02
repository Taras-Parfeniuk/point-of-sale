using System;
using System.Collections.Generic;
using System.Linq;
using PointOfSale.SaleItems;

namespace PointOfSale
{
    public class PointOfSaleTerminalSession
    {
        private readonly ISaleItemDataProvider m_dataStore;
        private readonly Dictionary<string, SaleItemCheckEntry> m_check;

        public PointOfSaleTerminalSession(ISaleItemDataProvider dataStore)
        {
            m_dataStore = dataStore;
            m_check = new Dictionary<string, SaleItemCheckEntry>();
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
                (total, entry) => total + entry.Value.CalculatePrice()
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

            public decimal CalculatePrice()
            {
                if (Count == 0) return 0;

                decimal total;
                if (Item.HasDiscount)
                {
                    var discountVolumesCount = Count / Item.Discount.Count;

                    total = discountVolumesCount * Item.Discount.Price + (Count - discountVolumesCount * Item.Discount.Count) * Item.Price;
                }
                else
                {
                    total = Count * Item.Price;
                }

                return total;
            }
        }
    }
}
