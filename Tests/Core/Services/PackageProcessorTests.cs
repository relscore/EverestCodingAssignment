using CourierService.Core.Models;
using CourierService.Core.Services;
using CourierService.Core.Services.Delivery;
using Moq;
using Xunit;

namespace CourierService.Tests.Core.Services
{
    public class PackageProcessorTests
    {
        private readonly Mock<IDeliveryCostCalculator> _mockCostCalculator;
        private readonly Mock<IDeliveryTimeCalculator> _mockTimeCalculator;
        private readonly PackageProcessor _processor;

        public PackageProcessorTests()
        {
            _mockCostCalculator = new Mock<IDeliveryCostCalculator>();
            _mockTimeCalculator = new Mock<IDeliveryTimeCalculator>();
            _processor = new PackageProcessor(_mockCostCalculator.Object, _mockTimeCalculator.Object);
        }

        [Fact]
        public void ProcessPackagesForDeliveryCost_ReturnsResultsForAllPackages()
        {
            // Arrange
            var packages = new List<Package>
            {
                new Package { Id = "PKG1" },
                new Package { Id = "PKG2" }
            };

            var expectedResults = new List<DeliveryResult>
            {
                new DeliveryResult { PackageId = "PKG1", TotalCost = 750 },
                new DeliveryResult { PackageId = "PKG2", TotalCost = 1475 }
            };

            _mockCostCalculator.SetupSequence(x => x.CalculateDeliveryCost(It.IsAny<Package>()))
                .Returns(expectedResults[0])
                .Returns(expectedResults[1]);

            // Act
            var results = _processor.ProcessPackagesForDeliveryCost(packages);

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Equal("PKG1", results[0].PackageId);
            Assert.Equal(750, results[0].TotalCost);
            Assert.Equal("PKG2", results[1].PackageId);
            Assert.Equal(1475, results[1].TotalCost);
        }

        [Fact]
        public void ProcessPackagesWithDeliveryTime_DelegatesToTimeCalculator()
        {
            // Arrange
            var packages = new List<Package>
            {
                new Package { Id = "PKG1" },
                new Package { Id = "PKG2" }
            };

            int vehicles = 2;
            decimal maxSpeed = 70;
            decimal maxWeight = 200;

            var expectedResults = new List<DeliveryResult>
            {
                new DeliveryResult { PackageId = "PKG1", TotalCost = 750, EstimatedDeliveryTime = 4.0m },
                new DeliveryResult { PackageId = "PKG2", TotalCost = 1475, EstimatedDeliveryTime = 1.79m }
            };

            _mockTimeCalculator.Setup(x => x.CalculateDeliveryTimes(
                packages, vehicles, maxSpeed, maxWeight))
                .Returns(expectedResults);

            // Act
            var results = _processor.ProcessPackagesWithDeliveryTime(
                packages, vehicles, maxSpeed, maxWeight);

            // Assert
            Assert.Equal(2, results.Count);
            _mockTimeCalculator.Verify(x => x.CalculateDeliveryTimes(
                packages, vehicles, maxSpeed, maxWeight), Times.Once);
        }

        [Fact]
        public void ProcessPackagesForDeliveryCost_EmptyList_ReturnsEmptyResults()
        {
            // Arrange
            var emptyList = new List<Package>();

            // Act
            var results = _processor.ProcessPackagesForDeliveryCost(emptyList);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void ProcessPackagesWithDeliveryTime_EmptyList_ReturnsEmptyResults()
        {
            // Arrange
            var emptyList = new List<Package>();

            // Act
            var results = _processor.ProcessPackagesWithDeliveryTime(emptyList, 2, 70, 200);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);

            // Bug Fix: When list is empty, CalculateDeliveryTimes should not be called
            _mockTimeCalculator.Verify(
                x => x.CalculateDeliveryTimes(
                    It.IsAny<List<Package>>(), 
                    It.IsAny<int>(), 
                    It.IsAny<decimal>(), 
                    It.IsAny<decimal>()),
                Times.Never()); // Changed from Times.Once() to Times.Never()
            
            _mockCostCalculator.Verify(
                x => x.CalculateDeliveryCost(It.IsAny<Package>()),
                Times.Never()); // Also shouldn't calculate costs for empty list
        }
    }
}
