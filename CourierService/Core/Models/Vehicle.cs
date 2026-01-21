namespace CourierService.Core.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public decimal AvailableAfter { get; set; }
        public List<Package> CurrentLoad { get; set; } = new List<Package>();
        
        public bool CanCarry(decimal weight)
        {
            return CurrentLoad.Sum(p => p.Weight) + weight <= VehicleFleet.MaxCarriableWeight;
        }
    }

    public static class VehicleFleet
    {
        public static int VehicleCount { get; set; }
        public static decimal MaxSpeed { get; set; }
        public static decimal MaxCarriableWeight { get; set; }
        public static List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
