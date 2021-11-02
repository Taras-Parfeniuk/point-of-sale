using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PointOfSale.Discounts;
using PointOfSale.SaleItems;

namespace PointOfSale.Tests
{
    [TestFixture]
    public class PointOfSaleTerminalSessionTests
    {
        const string NONEXISTING_PRODUCT_CODE = "X";

        private static readonly Guid s_emptyDiscountCardId = Guid.NewGuid();
        private static readonly Guid s_chargedDiscountCardId = Guid.NewGuid();

        private MockRepository m_mockRepository;
        private Mock<ISaleItemDataProvider> m_dataProviderMock;
        private Mock<IDiscountCardStorage> m_cardStorageMock;

        private IDiscountCardStorage m_cardStorage;
        private ISaleItemDataProvider m_dataProvider;

        [SetUp]
        public void Setup()
        {
            m_mockRepository = new MockRepository(MockBehavior.Strict);
            m_dataProviderMock = m_mockRepository.Create<ISaleItemDataProvider>();
            m_cardStorageMock = m_mockRepository.Create<IDiscountCardStorage>();

            foreach (var item in GetTestSaleItems())
            {
                m_dataProviderMock
                    .Setup(s => s.GetByCode(item.Code))
                    .Returns(item);
            }

            m_dataProviderMock
                .Setup(s => s.GetByCode(NONEXISTING_PRODUCT_CODE))
                .Returns(default(SaleItem));

            m_dataProvider = m_dataProviderMock.Object;

            var emptyCard = new DiscountCard(s_emptyDiscountCardId, 0, GetDiscountRules());

            m_cardStorageMock
                .Setup(s => s.GetCardById(s_emptyDiscountCardId))
                .Returns(emptyCard);

            m_cardStorageMock
                .Setup(s => s.Update(emptyCard))
                .Verifiable();

            m_cardStorageMock
                .Setup(s => s.Save())
                .Verifiable();

            var chargedCard = new DiscountCard(s_chargedDiscountCardId, 10000, GetDiscountRules());

            m_cardStorageMock
                .Setup(s => s.GetCardById(s_chargedDiscountCardId))
                .Returns(chargedCard);

            m_cardStorage = m_cardStorageMock.Object;
        }

        [Test]
        [TestCase("ABCDABA", 13.25)]
        [TestCase("CCCCCCC", 6.00)]
        [TestCase("ABCD", 7.25)]
        [TestCase("ABCD" + NONEXISTING_PRODUCT_CODE, 7.25)]
        public void CalculateTotal_SequenceOfCodes_ShouldReturnCorrectTotal(string inputCodes, decimal expectedTotal)
        {
            var target = new PointOfSaleTerminalSession(m_dataProvider, m_cardStorage);

            var codes = inputCodes.Select(c => c.ToString());

            foreach (var code in codes)
            {
                target.Scan(code);
            }

            var total = target.CalculateTotal();

            Assert.AreEqual(expectedTotal, total);
        }

        [Test]
        [TestCase("ABCDABA", 14)]
        [TestCase("CCCCCCC", 7)]
        [TestCase("ABCD", 7.25)]
        [TestCase("ABCD" + NONEXISTING_PRODUCT_CODE, 7.25)]
        public void CloseSession_SequenceOfCodesWithGivenEmptyCard_ShouldChargeCardWithFullAmount(string inputCodes, decimal expectedCardAmount)
        {
            var cardId = s_emptyDiscountCardId;
            var target = new PointOfSaleTerminalSession(m_dataProvider, m_cardStorage);
            var card = m_cardStorage.GetCardById(cardId);
            var codes = inputCodes.Select(c => c.ToString());

            foreach (var code in codes)
            {
                target.Scan(code);
            }

            target.UseDiscountCard(cardId);

            target.CloseSession();

            Assert.AreEqual(expectedCardAmount, card.Amount);
        }

        [Test]
        public void CloseSession_GivenCard_ShouldCallUpdateAndSaveOnCardStorage()
        {
            var cardId = s_emptyDiscountCardId;
            var target = new PointOfSaleTerminalSession(m_dataProvider, m_cardStorage);
            
            target.UseDiscountCard(cardId);

            target.CloseSession();

            m_mockRepository.Verify();
        }

        [Test]
        public void CloseSession_GivenEmptyCard_ShouldChargeCardOnlyOnce()
        {
            var cardId = s_emptyDiscountCardId;
            var card = m_cardStorage.GetCardById(s_emptyDiscountCardId);
            var target = new PointOfSaleTerminalSession(m_dataProvider, m_cardStorage);

            target.Scan("C");
            target.UseDiscountCard(cardId);

            target.CloseSession();
            target.CloseSession();

            Assert.AreEqual(1, card.Amount);
        }

        [Test]
        [TestCase("ABCDABAA", 13.695)]
        [TestCase("CCCCCCC", 5.93)]
        [TestCase("ABCD", 6.7425)]
        [TestCase("ABCD" + NONEXISTING_PRODUCT_CODE, 6.7425)]
        public void CalculateTotal_SequenceOfCodesWithGivenChargedCard_ShouldReturnCorrectTotal(string inputCodes, decimal expectedTotal)
        {
            var cardId = s_chargedDiscountCardId;
            var target = new PointOfSaleTerminalSession(m_dataProvider, m_cardStorage);
            var card = m_cardStorage.GetCardById(cardId);
            var codes = inputCodes.Select(c => c.ToString());

            foreach (var code in codes)
            {
                target.Scan(code);
            }

            target.UseDiscountCard(cardId);

            var total = target.CalculateTotal();

            Assert.AreEqual(expectedTotal, total);
        }

        private IEnumerable<SaleItem> GetTestSaleItems()
        {
            var itemA = new SaleItem("A", new decimal(1.25));
            itemA.SetPrice(3, new decimal(3.00));
            yield return itemA;

            var itemB = new SaleItem("B", new decimal(4.25));
            yield return itemB;

            var itemC = new SaleItem("C", new decimal(1.00));
            itemC.SetPrice(6, new decimal(5.00));
            yield return itemC;

            var itemD = new SaleItem("D", new decimal(0.75));
            yield return itemD;
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
