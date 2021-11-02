using System;
using System.Collections.Generic;
using NUnit.Framework;
using PointOfSale.Discounts;

namespace PointOfSale.Tests
{
    [TestFixture]
    public class DiscountCardTests
    {
        [Test]
        public void ApplyDiscount_NoMatchingRules_ShouldReturnInitialPrice()
        {
            var target = new DiscountCard(Guid.NewGuid(), 0, GetDiscountRules());

            decimal input = 100;
            decimal expected = input;

            var actual = target.ApplyDiscount(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ApplyDiscount_MatchLastGivenRule_ShouldCalculateCorrectPrice()
        {
            var target = new DiscountCard(Guid.NewGuid(), 10000, GetDiscountRules());

            decimal input = 100;
            decimal expected = 93;

            var actual = target.ApplyDiscount(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ApplyDiscount_MatchTwoRules_ShouldCalculatePriceUsingFirstRule()
        {
            var target = new DiscountCard(Guid.NewGuid(), 1999, GetDiscountRules());

            decimal input = 100;
            decimal expected = 99;

            var actual = target.ApplyDiscount(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(20, 100, 100)]
        [TestCase(1000, 100, 99)]
        [TestCase(4999, 100, 97)]
        [TestCase(4999.01, 100, 95)]
        public void ApplyDiscount_GivenCardAmounts_ShouldCalculatePriceUsingCorrectRule(decimal cardAmount, decimal inputPrice, decimal expectedTotal)
        {
            var target = new DiscountCard(Guid.NewGuid(), cardAmount, GetDiscountRules());

            decimal input = inputPrice;
            decimal expected = expectedTotal;

            var actual = target.ApplyDiscount(input);

            Assert.AreEqual(expected, actual);
        }

        private IEnumerable<CardAmountDiscountRule> GetDiscountRules()
        {
            yield return new CardAmountDiscountRule(1000, 1999, 1);
            yield return new CardAmountDiscountRule(1999, 4999, 3);
            yield return new CardAmountDiscountRule(4999, 9999, 5);
            yield return new CardAmountDiscountRule(9999, default, 7);
        }
    }
}
