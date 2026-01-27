using CourierService.Core.Models;
using CourierService.Core.Services.Delivery;

namespace CourierService.Core.Services
{
    public class PackageProcessor : IPackageProcessor
    {
        private readonly IDeliveryCostCalculator _costCalculator;
        private readonly IDeliveryTimeCalculator _timeCalculator;

        public PackageProcessor(IDeliveryCostCalculator costCalculator, IDeliveryTimeCalculator timeCalculator)
        {
            _costCalculator = costCalculator;
            _timeCalculator = timeCalculator;
        }

        public List<DeliveryResult> ProcessPackagesForDeliveryCost(List<Package> packages)
        {
            return packages.Select(_costCalculator.CalculateDeliveryCost).ToList();
        }

        public List<DeliveryResult> ProcessPackagesWithDeliveryTime(List<Package> packages, int vehicles, decimal maxSpeed, decimal maxCarriableWeight)
        {
            if (packages == null || !packages.Any())
            {
                return new List<DeliveryResult>();
            }

            return _timeCalculator.CalculateDeliveryTimes(packages, vehicles, maxSpeed, maxCarriableWeight);
        }
    }
}
