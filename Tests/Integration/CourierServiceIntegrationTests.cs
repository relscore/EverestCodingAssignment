using CourierService.Core.Models;
using CourierService.Core.Services;
using CourierService.Core.Services.Delivery;
using CourierService.Infrastructure.Offers;
using CourierService.Infrastructure.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CourierService.Tests.Integration
{
    public class CourierServiceIntegrationTests
    {
        private readonly ServiceProvider _serviceProvider;

        public CourierServiceIntegrationTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddSingleton<IOfferService, OfferService>()
                .AddSingleton<IDeliveryCostCalculator, DeliveryCostCalculator>()
                .AddSingleton<IDeliveryTimeCalculator, DeliveryTimeCalculator>()
                .AddSingleton<IInputParser, InputParser>()
                .AddSingleton<IPackageProcessor, PackageProcessor>()
                .BuildServiceProvider();
        }

        [Fact]
        public void FullPipeline_DeliveryCostOnly_ReturnsCorrectResults()
        {
            // Arrange
            var parser = _serviceProvider.GetService<IInputParser>();
            var processor = _serviceProvider.GetService<IPackageProcessor>();
            string input = "100 3 PKG1 5 5 OFR001 PKG2 15 5 OFR002 PKG3 10 100 OFR003";

            // Act
            var packages = parser.ParseDeliveryCostInput(input);
            var results = processor.ProcessPackagesForDeliveryCost(packages);

            // Assert
            Assert.Equal(3, results.Count);
            
            Assert.Equal("PKG1", results[0].PackageId);
            Assert.Equal(0, results[0].Discount);
            Assert.Equal(175, results[0].TotalCost);
            
            Assert.Equal("PKG2", results[1].PackageId);
            Assert.Equal(0, results[1].Discount);
            Assert.Equal(275, results[1].TotalCost);
            
            Assert.Equal("PKG3", results[2].PackageId);
            Assert.Equal(35, results[2].Discount);
            Assert.Equal(665, results[2].TotalCost);
        }

        [Fact]
        public void FullPipeline_DeliveryCostWithDiscounts_ReturnsCorrectResults()
        {
            // Arrange
            var parser = _serviceProvider.GetService<IInputParser>();
            var processor = _serviceProvider.GetService<IPackageProcessor>();
            string input = "100 2 PKG1 100 150 OFR001 PKG4 110 60 OFFR002";

            // Act
            var packages = parser.ParseDeliveryCostInput(input);
            var results = processor.ProcessPackagesForDeliveryCost(packages);

            // Assert
            Assert.Equal(2, results.Count);
            
            Assert.Equal("PKG1", results[0].PackageId);
            Assert.Equal(185, results[0].Discount);
            Assert.Equal(1665, results[0].TotalCost);
            
            Assert.Equal("PKG4", results[1].PackageId);
            Assert.Equal(105, results[1].Discount);
            Assert.Equal(1395, results[1].TotalCost);
        }

        [Theory]
        [InlineData("OFR001", 150, 100, 1850, 185)]
        [InlineData("OFR002", 100, 150, 2100, 147)]
        [InlineData("OFR003", 100, 100, 1600, 80)]
        [InlineData("OFR001", 250, 100, 2100, 0)]
        public void OfferService_Integration_CalculatesCorrectDiscount(
            string offerCode, decimal distance, decimal weight, 
            decimal deliveryCost, decimal expectedDiscount)
        {
            // Arrange
            var offerService = _serviceProvider.GetService<IOfferService>();

            // Act
            decimal discount = offerService.CalculateDiscount(
                offerCode, distance, weight, deliveryCost);

            // Assert
            Assert.Equal(expectedDiscount, discount);
        }
    }
}
