namespace CourierService.Core.Models
{
    public class DeliveryResult
    {
        public string PackageId { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalCost { get; set; }
        public decimal EstimatedDeliveryTime { get; set; }
        public decimal DeliveryCostBeforeDiscount { get; set; }
    }
}
