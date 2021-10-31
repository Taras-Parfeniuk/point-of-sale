using System.Collections.Generic;
using System.Linq;
using PointOfSale.Results;
using PointOfSale.Storage;

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

        public Result Scan(string code)
        {
            if (m_check.TryGetValue(code, out SaleItemCheckEntry entry))
            {
                entry.Increment();

                return Result.SuccessResult();
            }

            return m_dataStore.GetByCode(code)
                .Apply(ScanNewItem);
        }
 
        private Result ScanNewItem(SaleItem item)
        {
            if (item == default || item.Code == default) return Result.ErrorResult("Unable to scan item. Value cannot be null.");

            if (m_check.TryAdd(item.Code, new SaleItemCheckEntry(item, 1))) return Result.SuccessResult();

            return Result.ErrorResult("Unable to scan item.");
        }

        public Result<decimal> CalculateTotal()
        {
            return m_check.Aggregate
            (
                Result<decimal>.SuccessResult(0), 
                (total, entry) => entry.Value.CalculatePrice()
                    .Apply(price => Result<decimal>.SuccessResult(total.Value + price))
            );
        }

        class SaleItemCheckEntry
        {
            public SaleItemCheckEntry(SaleItem item, int count)
            {
                Item = item;
                Count = count;
            }

            public SaleItem Item { get; }
            public int Count { get; private set; }

            public void Increment()
            {
                Count++;
            }

            public void Decrement()
            {
                if (Count < 0) Count--;
            }

            public Result<decimal> CalculatePrice()
            {
                if (Count < 0) return Result<decimal>.ErrorResult("Failed to calculate price. Count cannot be less then 0.");

                if (Count == 0) return Result<decimal>.SuccessResult(0);

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

                return Result<decimal>.SuccessResult(total);
            }
        }
    }
}
