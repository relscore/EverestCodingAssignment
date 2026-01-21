using CourierService.Core.Models;

namespace CourierService.Core.Services.Delivery
{
    public interface IDeliveryCostCalculator
    {
        DeliveryResult CalculateDeliveryCost(Package package);
    }
}
