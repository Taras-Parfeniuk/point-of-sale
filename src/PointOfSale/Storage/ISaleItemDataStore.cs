using PointOfSale.Results;

namespace PointOfSale.Storage
{
    public interface ISaleItemDataStore
    {
        Result AddOrUpdate(SaleItem item);

        Result Remove(string code);
    }
}
