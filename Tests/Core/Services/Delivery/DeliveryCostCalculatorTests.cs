using CourierService.Core.Models;
using CourierService.Core.Services;
using CourierService.Core.Services.Delivery;
using Moq;
using Xunit;

namespace CourierService.Tests.Core.Services.Delivery
{
    public class DeliveryCostCalculatorTests
    {
        private readonly Mock<IOfferService> _mockOfferService;
        private readonly DeliveryCostCalculator _calculator;

        public DeliveryCostCalculatorTests()
        {
            _mockOfferService = new Mock<IOfferService>();
            _calculator = new DeliveryCostCalculator(_mockOfferService.Object);
        }

        [Fact]
        public void CalculateDeliveryCost_ReturnsCorrectCostWithoutDiscount()
        {
            // Arrange
            var package = new Package
            {
                Id = "PKG1",
                Weight = 50,
                Distance = 100,
                OfferCode = "OFR001",
                BaseDeliveryCost = 100
            };

            _mockOfferService.Setup(x => x.CalculateDiscount(
                "OFR001", 100, 50, It.IsAny<decimal>()))
                .Returns(0);

            // Act
            var result = _calculator.CalculateDeliveryCost(package);

            // Assert
            Assert.Equal("PKG1", result.PackageId);
            Assert.Equal(0, result.Discount);
            Assert.Equal(1100, result.TotalCost);
            Assert.Equal(1100, result.DeliveryCostBeforeDiscount);
            Assert.Equal(0, result.EstimatedDeliveryTime);
        }

        [Fact]
        public void CalculateDeliveryCost_ReturnsCorrectCostWithDiscount()
        {
            // Arrange
            var package = new Package
            {
                Id = "PKG2",
                Weight = 110,
                Distance = 60,
                OfferCode = "OFFR002",
                BaseDeliveryCost = 100
            };

            _mockOfferService.Setup(x => x.CalculateDiscount(
                "OFFR002", 60, 110, 1500))
                .Returns(105);

            // Act
            var result = _calculator.CalculateDeliveryCost(package);

            // Assert
            Assert.Equal("PKG2", result.PackageId);
            Assert.Equal(105, result.Discount);
            Assert.Equal(1395, result.TotalCost);
            Assert.Equal(1500, result.DeliveryCostBeforeDiscount);
        }

        [Fact]
        public void CalculateDeliveryCost_RoundsToTwoDecimalPlaces()
        {
            // Arrange
            var package = new Package
            {
                Id = "PKG3",
                Weight = 33.33m,
                Distance = 66.66m,
                OfferCode = "OFR003",
                BaseDeliveryCost = 99.99m
            };

            _mockOfferService.Setup(x => x.CalculateDiscount(
                It.IsAny<string>(), It.IsAny<decimal>(), 
                It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(123.456m);

            // Act
            var result = _calculator.CalculateDeliveryCost(package);

            // Assert
            Assert.Equal(123.46m, result.Discount);
            Assert.Equal(Math.Round(result.TotalCost, 2), result.TotalCost);
        }

        [Fact]
        public void CalculateDeliveryCost_ThrowsExceptionForNullPackage()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _calculator.CalculateDeliveryCost(null));
        }
    }
}
