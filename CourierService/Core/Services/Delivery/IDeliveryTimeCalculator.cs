using CourierService.Core.Models;

namespace CourierService.Core.Services.Delivery
{
    public interface IDeliveryTimeCalculator
    {
        List<DeliveryResult> CalculateDeliveryTimes(List<Package> packages, int vehicleCount, decimal maxSpeed, decimal maxCarriableWeight);
    }
}
