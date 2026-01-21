using CourierService.Core.Models;
using CourierService.Core.Services;

namespace CourierService.Core.Services.Delivery
{
    public class DeliveryCostCalculator : IDeliveryCostCalculator
    {
        private readonly IOfferService _offerService;

        public DeliveryCostCalculator(IOfferService offerService)
        {
            _offerService = offerService;
        }

        public DeliveryResult CalculateDeliveryCost(Package package)
        {
            // Calculate delivery cost: Base Cost + (Weight * 10) + (Distance * 5)
            var deliveryCost = package.BaseDeliveryCost + 
                              (package.Weight * 10) + 
                              (package.Distance * 5);

            // Calculate discount if applicable
            var discount = _offerService.CalculateDiscount(
                package.OfferCode, 
                package.Distance, 
                package.Weight, 
                deliveryCost
            );

            var totalCost = deliveryCost - discount;

            return new DeliveryResult
            {
                PackageId = package.Id,
                Discount = Math.Round(discount, 2),
                TotalCost = Math.Round(totalCost, 2),
                DeliveryCostBeforeDiscount = Math.Round(deliveryCost, 2),
                EstimatedDeliveryTime = 0
            };
        }
    }
}
