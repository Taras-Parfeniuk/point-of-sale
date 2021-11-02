using System;

namespace PointOfSale.SaleItems
{
    public class SaleItem
    {
        public SaleItem(string code, decimal price)
        {
            Code = code;
            Price = price;
        }

        public string Code { get; }

        public decimal Price { get; private set; }

        public SaleItemVolumePrice VolumePrice { get; private set; }

        public bool HasVolumePrice => VolumePrice != null;

        public void SetPrice(int count, decimal price)
        {

            if (count < 1) throw new ArgumentException("Value cannot be less then 1.", nameof(count));
            if (price < 0) throw new ArgumentException("Value cannot be less then 0.", nameof(price));

            if (count == 1)
            {
                Price = price;
            }
            else
            {
                VolumePrice = new SaleItemVolumePrice(count, price);
            }
        }
    }
}
