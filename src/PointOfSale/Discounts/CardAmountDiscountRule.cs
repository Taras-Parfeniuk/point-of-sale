using PointOfSale.Common;

namespace PointOfSale.Discounts
{
    public class CardAmountDiscountRule
    {
        private readonly Range m_cardAmountRange;
        private readonly decimal m_discountPercent;

        public CardAmountDiscountRule(decimal? amountFrom, decimal? amountTo, decimal discountPercent)
        {
            m_cardAmountRange = new Range(amountFrom, amountTo);
            m_discountPercent = discountPercent;
        }

        public bool CanApply(DiscountCard card)
        {
            return m_cardAmountRange.IsInRange(card.Amount);
        }

        public decimal Apply(decimal price)
        {
            return price * (100 - m_discountPercent) / 100;
        }
    }
}
