namespace PointOfSale
{
    public class SaleItemDiscount
    {
        public SaleItemDiscount(int count, decimal price)
        {
            Count = count;
            Price = price;
        }

        public int Count { get; }

        public decimal Price { get; }
    }
}
