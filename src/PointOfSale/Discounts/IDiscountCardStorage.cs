using System;

namespace PointOfSale.Discounts
{
    public interface IDiscountCardStorage
    {
        DiscountCard GetCardById(Guid id);
        void Update(DiscountCard m_discountCard);
        void Save();
    }
}
