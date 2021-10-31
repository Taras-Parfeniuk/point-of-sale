using PointOfSale.Results;

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

        public Result SetPrice(int count, decimal price)
        {
            if (count < 1) return Result.ErrorResult("Invalid count. Value cannot be less then 1.");

            if (count == 1)
            {
                Price = price;
            }
            else
            {
                Discount = new SaleItemDiscount(count, price);
            }

            return Result.SuccessResult();
        }
    }
}
