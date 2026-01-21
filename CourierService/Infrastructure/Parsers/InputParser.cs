using CourierService.Core.Models;

namespace CourierService.Infrastructure.Parsers
{
    public interface IInputParser
    {
        List<Package> ParseDeliveryCostInput(string input);
        (List<Package> packages, int vehicles, decimal maxSpeed, decimal maxCarriableWeight) ParseDeliveryTimeInput(string input);
    }

    public class InputParser : IInputParser
    {
        public List<Package> ParseDeliveryCostInput(string input)
        {
            var packages = new List<Package>();
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 3) 
                throw new ArgumentException("Invalid input format");

            var baseCost = decimal.Parse(parts[0]);
            var packageCount = int.Parse(parts[1]);

             // Validate we have enough parts
            int expectedParts = 2 + (packageCount * 4);
            if (parts.Length < expectedParts)
                throw new ArgumentException($"Invalid input: Expected {expectedParts} parts but got {parts.Length}");

            for (int i = 0; i < packageCount; i++)
            {
                var index = 2 + (i * 4);
                var package = new Package
                {
                    Id = parts[index],
                    Weight = decimal.Parse(parts[index + 1]),
                    Distance = decimal.Parse(parts[index + 2]),
                    OfferCode = parts[index + 3],
                    BaseDeliveryCost = baseCost
                };
                packages.Add(package);
            }

            return packages;
        }

        public (List<Package> packages, int vehicles, decimal maxSpeed, decimal maxCarriableWeight) ParseDeliveryTimeInput(string input)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            var baseCost = decimal.Parse(parts[0]);
            var packageCount = int.Parse(parts[1]);
            
            // Validate we have enough parts for all packages
            int packagePartsEndIndex = 2 + (packageCount * 4);
            if (parts.Length < packagePartsEndIndex + 3)
                throw new ArgumentException($"Invalid input: Expected {packagePartsEndIndex + 3} parts for {packageCount} packages but got {parts.Length}");
                
            var packages = new List<Package>();
            
            for (int i = 0; i < packageCount; i++)
            {
                var index = 2 + (i * 4);
                var package = new Package
                {
                    Id = parts[index],
                    Weight = decimal.Parse(parts[index + 1]),
                    Distance = decimal.Parse(parts[index + 2]),
                    OfferCode = parts[index + 3],
                    BaseDeliveryCost = baseCost
                };
                packages.Add(package);
            }
            
            var vehicleIndex = 2 + (packageCount * 4);
            var vehicles = int.Parse(parts[vehicleIndex]);
            var maxSpeed = decimal.Parse(parts[vehicleIndex + 1]);
            var maxCarriableWeight = decimal.Parse(parts[vehicleIndex + 2]);
            
            return (packages, vehicles, maxSpeed, maxCarriableWeight);
        }
    }
}
