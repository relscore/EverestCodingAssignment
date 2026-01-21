namespace CourierService.Core.Models
{
    public class Offer
    {
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public DistanceRange DistanceRange { get; set; }
        public WeightRange WeightRange { get; set; }
    }

    public class DistanceRange
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }

    public class WeightRange
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }
}
