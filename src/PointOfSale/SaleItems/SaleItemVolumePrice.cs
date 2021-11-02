namespace PointOfSale.SaleItems
{
    public class SaleItemVolumePrice
    {
        public SaleItemVolumePrice(int count, decimal price)
        {
            Count = count;
            Price = price;
        }

        public int Count { get; }

        public decimal Price { get; }
    }
}
