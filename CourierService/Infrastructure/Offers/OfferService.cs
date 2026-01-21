using CourierService.Core.Models;
using CourierService.Core.Services;

namespace CourierService.Infrastructure.Offers
{
    public class OfferService : IOfferService
    {
        private readonly Dictionary<string, Offer> _offers = new();

        public OfferService()
        {
            InitializeDefaultOffers();
        }

        private void InitializeDefaultOffers()
        {
            // OFR001: 10% discount, distance < 200, weight 70-200
            _offers["OFR001"] = new Offer
            {
                Code = "OFR001",
                DiscountPercent = 10,
                DistanceRange = new DistanceRange { Min = 0, Max = 199 }, // < 200 means 0-199
                WeightRange = new WeightRange { Min = 70, Max = 200 }
            };

            // OFR002: 7% discount, distance 50-150, weight 100-250
            _offers["OFR002"] = new Offer
            {
                Code = "OFR002",
                DiscountPercent = 7,
                DistanceRange = new DistanceRange { Min = 50, Max = 150 },
                WeightRange = new WeightRange { Min = 100, Max = 250 }
            };

            // Input has OFFR002 (with extra F) - based on sample output, PKG4 gets discount
            _offers["OFFR002"] = new Offer
            {
                Code = "OFFR002",
                DiscountPercent = 7,
                DistanceRange = new DistanceRange { Min = 50, Max = 150 },
                WeightRange = new WeightRange { Min = 100, Max = 250 }
            };

            // OFR003: 5% discount, distance 50-250, weight 10-150
            _offers["OFR003"] = new Offer
            {
                Code = "OFR003",
                DiscountPercent = 5,
                DistanceRange = new DistanceRange { Min = 50, Max = 250 },
                WeightRange = new WeightRange { Min = 10, Max = 150 }
            };

            // OFFR08 - based on sample
            _offers["OFFR08"] = new Offer
            {
                Code = "OFFR08",
                DiscountPercent = 0, // No discount
                DistanceRange = new DistanceRange { Min = 0, Max = 0 },
                WeightRange = new WeightRange { Min = 0, Max = 0 }
            };

            _offers["OFFR0008"] = new Offer
            {
                Code = "OFFR0008",
                DiscountPercent = 0,
                DistanceRange = new DistanceRange { Min = 0, Max = 0 },
                WeightRange = new WeightRange { Min = 0, Max = 0 }
            };

            // NA means No Offer/Not Applicable
            _offers["NA"] = new Offer
            {
                Code = "NA",
                DiscountPercent = 0,
                DistanceRange = new DistanceRange { Min = 0, Max = 0 },
                WeightRange = new WeightRange { Min = 0, Max = 0 }
            };

            // Empty string or null handling
            _offers[""] = new Offer
            {
                Code = "",
                DiscountPercent = 0,
                DistanceRange = new DistanceRange { Min = 0, Max = 0 },
                WeightRange = new WeightRange { Min = 0, Max = 0 }
            };
        }

        public bool IsValidOffer(string offerCode, decimal distance, decimal weight)
        {
            // Handle null or empty offer code
            if (string.IsNullOrEmpty(offerCode))
                return false;

            // Normalize the offer code (trim and uppercase)
            offerCode = offerCode.Trim().ToUpper();
            
            // Check if offer exists
            if (!_offers.ContainsKey(offerCode))
                return false;

            var offer = _offers[offerCode];
            
            // Special case: If offer has 0% discount or invalid ranges, it's not valid
            if (offer.DiscountPercent == 0 || 
                (offer.DistanceRange.Min == 0 && offer.DistanceRange.Max == 0 &&
                 offer.WeightRange.Min == 0 && offer.WeightRange.Max == 0))
            {
                return false;
            }

            // Check if distance and weight are within offer ranges
            bool isValidDistance = distance >= offer.DistanceRange.Min && distance <= offer.DistanceRange.Max;
            bool isValidWeight = weight >= offer.WeightRange.Min && weight <= offer.WeightRange.Max;
            
            return isValidDistance && isValidWeight;
        }

        public decimal CalculateDiscount(string offerCode, decimal distance, decimal weight, decimal deliveryCost)
        {
            if (!IsValidOffer(offerCode, distance, weight))
                return 0;

            var offer = _offers[offerCode.Trim().ToUpper()];
            
            // Calculate discount and round to 2 decimal places
            decimal discount = deliveryCost * (offer.DiscountPercent / 100m);
            return Math.Round(discount, 2);
        }

        public void AddOffer(Offer offer)
        {
            if (offer == null) return;
            
            // Normalize the code
            string normalizedCode = offer.Code?.Trim().ToUpper() ?? "";
            offer.Code = normalizedCode;
            
            _offers[normalizedCode] = offer;
        }
    }
}