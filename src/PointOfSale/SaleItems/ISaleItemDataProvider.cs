namespace PointOfSale.SaleItems
{
    public interface ISaleItemDataProvider
    {
        SaleItem GetByCode(string code);
    }
}
