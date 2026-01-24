using CourierService.Core.Models;
using CourierService.Infrastructure.Offers;
using Xunit;

namespace CourierService.Tests.Infrastructure.Offers
{
    public class OfferServiceTests
    {
        private readonly OfferService _offerService;

        public OfferServiceTests()
        {
            _offerService = new OfferService();
        }

        [Theory]
        [InlineData("OFR001", 150, 100, true)]
        [InlineData("OFR001", 199, 70, true)]
        [InlineData("OFR001", 200, 100, false)]
        [InlineData("OFR001", 150, 50, false)]
        [InlineData("OFR001", 150, 250, false)]
        [InlineData("OFR002", 100, 150, true)]
        [InlineData("OFR002", 50, 100, true)]
        [InlineData("OFR002", 151, 150, false)]
        [InlineData("OFR002", 100, 90, false)]
        [InlineData("OFR003", 100, 50, true)]
        [InlineData("OFR003", 250, 150, true)]
        [InlineData("OFR003", 49, 50, false)]
        [InlineData("OFR003", 100, 9, false)]
        [InlineData("INVALID", 100, 100, false)]
        [InlineData("", 100, 100, false)]
        [InlineData(null, 100, 100, false)]
        [InlineData("OFFR002", 100, 120, true)]
        [InlineData("OFFR08", 100, 120, false)]
        [InlineData("NA", 100, 120, false)]
        public void IsValidOffer_ReturnsExpectedResult(
            string offerCode, decimal distance, decimal weight, bool expected)
        {
            // Act
            bool result = _offerService.IsValidOffer(offerCode, distance, weight);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("OFR001", 150, 100, 1850, 185)]
        [InlineData("OFR002", 100, 150, 2100, 147)]
        [InlineData("OFR003", 100, 100, 1600, 80)]
        [InlineData("OFR001", 250, 100, 2100, 0)]
        [InlineData("INVALID", 100, 100, 1000, 0)]
        [InlineData("OFFR002", 60, 110, 1500, 105)]
        [InlineData("OFFR08", 125, 75, 1475, 0)]
        public void CalculateDiscount_ReturnsExpectedAmount(
            string offerCode, decimal distance, decimal weight, 
            decimal deliveryCost, decimal expectedDiscount)
        {
            // Act
            decimal discount = _offerService.CalculateDiscount(
                offerCode, distance, weight, deliveryCost);

            // Assert
            Assert.Equal(expectedDiscount, discount);
        }

        [Fact]
        public void AddOffer_AddsNewOfferToCollection()
        {
            // Arrange
            var newOffer = new Offer
            {
                Code = "OFR004",
                DiscountPercent = 15,
                DistanceRange = new DistanceRange { Min = 100, Max = 300 },
                WeightRange = new WeightRange { Min = 50, Max = 150 }
            };

            // Act
            _offerService.AddOffer(newOffer);

            // Assert
            bool isValid = _offerService.IsValidOffer("OFR004", 200, 100);
            Assert.True(isValid);
            
            decimal discount = _offerService.CalculateDiscount("OFR004", 200, 100, 1000);
            Assert.Equal(150, discount);
        }

        [Fact]
        public void CalculateDiscount_RoundsToTwoDecimalPlaces()
        {
            // Arrange
            decimal deliveryCost = 1234.56m;
            
            // Act
            decimal discount = _offerService.CalculateDiscount("OFR001", 150, 100, deliveryCost);
            
            // Assert
            Assert.Equal(123.46m, discount);
        }
    }
}
