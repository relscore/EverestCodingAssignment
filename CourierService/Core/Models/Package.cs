namespace CourierService.Core.Models
{
    public class Package
    {
        public string Id { get; set; }
        public decimal Weight { get; set; }
        public decimal Distance { get; set; }
        public string OfferCode { get; set; }
        public decimal BaseDeliveryCost { get; set; }
    }
}
