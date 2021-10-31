using PointOfSale.Results;

namespace PointOfSale.Storage
{
    public interface ISaleItemDataProvider
    {
        Result<SaleItem> GetByCode(string code);
    }
}
