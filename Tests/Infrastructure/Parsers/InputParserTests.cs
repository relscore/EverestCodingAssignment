using CourierService.Core.Models;
using CourierService.Infrastructure.Parsers;
using Xunit;

namespace CourierService.Tests.Infrastructure.Parsers
{
    public class InputParserTests
    {
        private readonly InputParser _parser;

        public InputParserTests()
        {
            _parser = new InputParser();
        }

        [Fact]
        public void ParseDeliveryCostInput_ValidInput_ReturnsPackages()
        {
            // Arrange
            string input = "100 3 PKG1 5 5 OFR001 PKG2 15 5 OFR002 PKG3 10 100 OFR003";

            // Act
            var packages = _parser.ParseDeliveryCostInput(input);

            // Assert
            Assert.Equal(3, packages.Count);
            
            Assert.Equal("PKG1", packages[0].Id);
            Assert.Equal(5, packages[0].Weight);
            Assert.Equal(5, packages[0].Distance);
            Assert.Equal("OFR001", packages[0].OfferCode);
            Assert.Equal(100, packages[0].BaseDeliveryCost);
            
            Assert.Equal("PKG2", packages[1].Id);
            Assert.Equal(15, packages[1].Weight);
            Assert.Equal(5, packages[1].Distance);
            Assert.Equal("OFR002", packages[1].OfferCode);
            
            Assert.Equal("PKG3", packages[2].Id);
            Assert.Equal(10, packages[2].Weight);
            Assert.Equal(100, packages[2].Distance);
            Assert.Equal("OFR003", packages[2].OfferCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("100")]
        [InlineData("100 3 PKG1")]
        [InlineData("100 3 PKG1 5 5")]
        [InlineData("100 2 PKG1 5 5 OFR001")]
        public void ParseDeliveryCostInput_InvalidInput_ThrowsException(string input)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.ParseDeliveryCostInput(input));
        }

        [Fact]
        public void ParseDeliveryTimeInput_ValidInput_ReturnsAllData()
        {
            // Arrange
            string input = "100 2 PKG1 50 30 OFR001 PKG2 75 125 OFFR08 2 70 200";

            // Act
            var (packages, vehicles, maxSpeed, maxCarriableWeight) = 
                _parser.ParseDeliveryTimeInput(input);

            // Assert
            Assert.Equal(2, packages.Count);
            Assert.Equal(2, vehicles);
            Assert.Equal(70, maxSpeed);
            Assert.Equal(200, maxCarriableWeight);
            
            Assert.Equal("PKG1", packages[0].Id);
            Assert.Equal(50, packages[0].Weight);
            Assert.Equal(30, packages[0].Distance);
            Assert.Equal("OFR001", packages[0].OfferCode);
            
            Assert.Equal("PKG2", packages[1].Id);
            Assert.Equal(75, packages[1].Weight);
            Assert.Equal(125, packages[1].Distance);
            Assert.Equal("OFFR08", packages[1].OfferCode);
        }

        [Fact]
        public void ParseDeliveryTimeInput_FullExampleFromPDF()
        {
            // Arrange
            string input = "100 5 PKG1 50 30 OFR001 PKG2 75 125 OFFR08 " +
                         "PKG3 175 100 OFR003 PKG4 110 60 OFFR002 PKG5 155 95 NA 2 70 200";

            // Act
            var (packages, vehicles, maxSpeed, maxCarriableWeight) = 
                _parser.ParseDeliveryTimeInput(input);

            // Assert
            Assert.Equal(5, packages.Count);
            Assert.Equal(2, vehicles);
            Assert.Equal(70, maxSpeed);
            Assert.Equal(200, maxCarriableWeight);
            
            Assert.Equal("PKG1", packages[0].Id);
            Assert.Equal("PKG2", packages[1].Id);
            Assert.Equal("PKG3", packages[2].Id);
            Assert.Equal("PKG4", packages[3].Id);
            Assert.Equal("PKG5", packages[4].Id);
            
            Assert.Equal("NA", packages[4].OfferCode);
        }

        [Theory]
        [InlineData("100")]
        [InlineData("100 5 PKG1 50 30")]
        [InlineData("100 2 PKG1 50 30 OFR001 2 70")]
        [InlineData("100 3 PKG1 5 5 OFR001 PKG2 15 5 2 70 200")]
        public void ParseDeliveryTimeInput_InvalidInput_ThrowsException(string input)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.ParseDeliveryTimeInput(input));
        }

        [Fact]
        public void ParseDeliveryCostInput_HandlesDecimalValues()
        {
            // Arrange
            string input = "99.99 2 PKG1 10.5 20.75 OFFER1 PKG2 15.25 30.5 OFFER2";

            // Act
            var packages = _parser.ParseDeliveryCostInput(input);

            // Assert
            Assert.Equal(99.99m, packages[0].BaseDeliveryCost);
            Assert.Equal(10.5m, packages[0].Weight);
            Assert.Equal(20.75m, packages[0].Distance);
        }

        [Fact]
        public void ParseDeliveryTimeInput_HandlesExtraSpaces()
        {
            // Arrange
            string input = "  100   2   PKG1   50   30   OFR001   PKG2   75   125   OFFR08   2   70   200  ";

            // Act
            var (packages, vehicles, maxSpeed, maxCarriableWeight) = 
                _parser.ParseDeliveryTimeInput(input);

            // Assert
            Assert.Equal(2, packages.Count);
            Assert.Equal(2, vehicles);
            Assert.Equal(70, maxSpeed);
            Assert.Equal(200, maxCarriableWeight);
        }
    }
}
