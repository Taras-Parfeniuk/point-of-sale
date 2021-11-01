using System;

namespace PointOfSale
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

        public SaleItemDiscount Discount { get; private set; }

        public bool HasDiscount => Discount != null;

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
                Discount = new SaleItemDiscount(count, price);
            }
        }
    }
}
