using CourierService.Core.Models;

namespace CourierService.Core.Services
{
    public interface IPackageProcessor
    {
        List<DeliveryResult> ProcessPackagesForDeliveryCost(List<Package> packages);
        List<DeliveryResult> ProcessPackagesWithDeliveryTime(List<Package> packages, int vehicles, decimal maxSpeed, decimal maxCarriableWeight);
    }
}
