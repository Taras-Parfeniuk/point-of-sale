using System;
using NUnit.Framework;
using PointOfSale.Discounts;

namespace PointOfSale.Tests
{
    [TestFixture]
    public class CardAmountDiscountRuleTests
    {
        [Test]
        [TestCase(default, default, 0, true)]
        [TestCase(1, 10, 10, true)]
        [TestCase(1, 10, 1, true)]
        [TestCase(default, 10, 0, true)]
        [TestCase(1, default, 10, true)]
        [TestCase(1, default, 0, false)]
        [TestCase(default, 10, 11, false)]
        [TestCase(1, 10, 11, false)]
        [TestCase(1, 10, 0, false)]
        public void CanApply_GivenCardAmount_ShouldReturnTrueIfAmountIsInRange(decimal? from, decimal? to, decimal inputCardAmount, bool expectedAppliable)
        {
            var target = new CardAmountDiscountRule(from, to, 0);
            var card = new DiscountCard(Guid.NewGuid(), inputCardAmount, new[] { target });
            
            var actual = target.CanApply(card);

            Assert.AreEqual(expectedAppliable, actual);
        }

        [Test]
        public void Apply_GivenPrice_ShouldCalculateCorrectTotal()
        {
            var expectedTotal = 50;
            var target = new CardAmountDiscountRule(0, 1, 50);

            var actual = target.Apply(100);

            Assert.AreEqual(expectedTotal, actual);
        }
    }
}
