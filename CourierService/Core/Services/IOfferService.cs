using CourierService.Core.Models;

namespace CourierService.Core.Services
{
    public interface IOfferService
    {
        bool IsValidOffer(string offerCode, decimal distance, decimal weight);
        decimal CalculateDiscount(string offerCode, decimal distance, decimal weight, decimal deliveryCost);
        void AddOffer(Offer offer);
    }
}
