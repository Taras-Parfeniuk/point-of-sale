using System;
using System.Collections.Generic;
using System.Linq;

namespace PointOfSale.Discounts
{
    public class DiscountCard : IDiscountProvider
    {
        private readonly IEnumerable<CardAmountDiscountRule> m_discountRules;
        private decimal m_amount;

        public DiscountCard(Guid id, decimal amount, IEnumerable<CardAmountDiscountRule> discountRules)
        {
            Id = id;
            m_amount = amount;
            m_discountRules = discountRules;
        }

        public Guid Id { get; private set; }
        public decimal Amount => m_amount;

        public void Charge(decimal amount)
        {
            if (amount < 0) throw new ArgumentException("Value cannot be less than 0.", nameof(amount));

            m_amount += amount;
        }

        public decimal ApplyDiscount(decimal price)
        {
            var ruleToApply = m_discountRules.FirstOrDefault(r => r.CanApply(this));

            if (ruleToApply == default) return price;

            return ruleToApply.Apply(price);
        }
    }
}
