using CourierService.Core.Models;
using Xunit;

namespace CourierService.Tests.Core.Models
{
    public class PackageTests
    {
        [Fact]
        public void Package_Constructor_SetsProperties()
        {
           
            var package = new Package
            {
                Id = "PKG1",
                Weight = 50.5m,
                Distance = 100.2m,
                OfferCode = "OFR001",
                BaseDeliveryCost = 100
            };

            Assert.Equal("PKG1", package.Id);
            Assert.Equal(50.5m, package.Weight);
            Assert.Equal(100.2m, package.Distance);
            Assert.Equal("OFR001", package.OfferCode);
            Assert.Equal(100, package.BaseDeliveryCost);
        }
    }

    public class DeliveryResultTests
    {
        [Fact]
        public void DeliveryResult_Properties_SetCorrectly()
        {
            
            var result = new DeliveryResult
            {
                PackageId = "PKG1",
                Discount = 105.50m,
                TotalCost = 1395.50m,
                EstimatedDeliveryTime = 1.75m,
                DeliveryCostBeforeDiscount = 1500m
            };

            
            Assert.Equal("PKG1", result.PackageId);
            Assert.Equal(105.50m, result.Discount);
            Assert.Equal(1395.50m, result.TotalCost);
            Assert.Equal(1.75m, result.EstimatedDeliveryTime);
            Assert.Equal(1500m, result.DeliveryCostBeforeDiscount);
        }
    }

    public class OfferTests
    {
        [Fact]
        public void Offer_Constructor_SetsProperties()
        {
            
            var offer = new Offer
            {
                Code = "OFR001",
                DiscountPercent = 10,
                DistanceRange = new DistanceRange { Min = 0, Max = 199 },
                WeightRange = new WeightRange { Min = 70, Max = 200 }
            };

            
            Assert.Equal("OFR001", offer.Code);
            Assert.Equal(10, offer.DiscountPercent);
            Assert.Equal(0, offer.DistanceRange.Min);
            Assert.Equal(199, offer.DistanceRange.Max);
            Assert.Equal(70, offer.WeightRange.Min);
            Assert.Equal(200, offer.WeightRange.Max);
        }
    }

    public class VehicleTests
    {
        [Fact]
        public void Vehicle_CanCarry_ReturnsTrueWhenWeightAvailable()
        {
            
            var vehicle = new Vehicle { Id = 1 };
            VehicleFleet.MaxCarriableWeight = 200;
            vehicle.CurrentLoad.Add(new Package { Weight = 100 });

            bool canCarry = vehicle.CanCarry(50);

            Assert.True(canCarry);
        }

        [Fact]
        public void Vehicle_CanCarry_ReturnsFalseWhenWeightExceeded()
        {
            var vehicle = new Vehicle { Id = 1 };
            VehicleFleet.MaxCarriableWeight = 200;
            vehicle.CurrentLoad.Add(new Package { Weight = 180 });

            bool canCarry = vehicle.CanCarry(30);

            Assert.False(canCarry);
        }
    }
}
