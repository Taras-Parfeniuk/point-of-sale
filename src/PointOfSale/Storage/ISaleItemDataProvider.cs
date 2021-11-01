namespace PointOfSale.Storage
{
    public interface ISaleItemDataProvider
    {
        SaleItem GetByCode(string code);
    }
}
