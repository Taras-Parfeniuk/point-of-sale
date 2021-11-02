namespace PointOfSale.Discounts
{
    interface IDiscountProvider
    {
        decimal ApplyDiscount(decimal price);
    }
}
