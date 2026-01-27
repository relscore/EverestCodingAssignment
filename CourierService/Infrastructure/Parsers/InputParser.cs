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
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be null or empty", nameof(input));

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length < 2)
                throw new ArgumentException(
                    "Input must contain at least base delivery cost and number of packages", 
                    nameof(input));

            // Parse base cost and package count with validation
            if (!decimal.TryParse(parts[0], out decimal baseCost))
                throw new ArgumentException(
                    $"Invalid base delivery cost: '{parts[0]}'", 
                    nameof(input));
            
            if (!int.TryParse(parts[1], out int packageCount))
                throw new ArgumentException(
                    $"Invalid package count: '{parts[1]}'", 
                    nameof(input));

            if (packageCount < 0)
                throw new ArgumentException(
                    "Package count cannot be negative", 
                    nameof(input));

            // Validate we have enough parts for all packages
            // Each package needs 4 parts: id, weight, distance, offerCode
            int expectedParts = 2 + (packageCount * 4);
            if (parts.Length < expectedParts)
                throw new ArgumentException(
                    $"Invalid input: Expected {expectedParts} parts for {packageCount} packages but got {parts.Length}",
                    nameof(input));

            var packages = new List<Package>();

            for (int i = 0; i < packageCount; i++)
            {
                var index = 2 + (i * 4);
                
                try
                {
                    var package = new Package
                    {
                        Id = ValidatePackageId(parts[index]),
                        Weight = ParsePositiveDecimal(parts[index + 1], "weight"),
                        Distance = ParsePositiveDecimal(parts[index + 2], "distance"),
                        OfferCode = ValidateOfferCode(parts[index + 3]),
                        BaseDeliveryCost = baseCost
                    };
                    
                    packages.Add(package);
                }
                catch (Exception ex) when (ex is ArgumentException or FormatException)
                {
                    throw new ArgumentException(
                        $"Error parsing package {i + 1}: {ex.Message}", 
                        nameof(input), ex);
                }
            }

            return packages;
        }

        public (List<Package> packages, int vehicles, decimal maxSpeed, decimal maxCarriableWeight) ParseDeliveryTimeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be null or empty", nameof(input));

            // Split by both newline and space to handle both formats
            var lines = input.Trim().Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .ToList();

            if (lines.Count == 0)
                throw new ArgumentException("Input contains no valid lines", nameof(input));

            // Combine all lines and split by spaces
            var allParts = new List<string>();
            foreach (var line in lines)
            {
                allParts.AddRange(line.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }

            var parts = allParts.ToArray();
            
            if (parts.Length < 2)
                throw new ArgumentException(
                    "Input must contain at least base delivery cost and number of packages", 
                    nameof(input));

            // Parse base cost and package count with validation
            if (!decimal.TryParse(parts[0], out decimal baseCost))
                throw new ArgumentException(
                    $"Invalid base delivery cost: '{parts[0]}'", 
                    nameof(input));
            
            if (!int.TryParse(parts[1], out int packageCount))
                throw new ArgumentException(
                    $"Invalid package count: '{parts[1]}'", 
                    nameof(input));

            if (packageCount < 0)
                throw new ArgumentException(
                    "Package count cannot be negative", 
                    nameof(input));

            // Validate we have enough parts for all packages
            // Each package needs 4 parts: id, weight, distance, offerCode
            // Plus 3 parts for vehicle info: vehicles, maxSpeed, maxCarriableWeight
            int expectedParts = 2 + (packageCount * 4) + 3;
            
            if (parts.Length < expectedParts)
                throw new ArgumentException(
                    $"Invalid input: Expected {expectedParts} parts " +
                    $"(2 base + {packageCount * 4} packages + 3 vehicle) but got {parts.Length}",
                    nameof(input));

            var packages = new List<Package>();

            for (int i = 0; i < packageCount; i++)
            {
                var index = 2 + (i * 4);
                
                try
                {
                    var package = new Package
                    {
                        Id = ValidatePackageId(parts[index]),
                        Weight = ParsePositiveDecimal(parts[index + 1], "weight"),
                        Distance = ParsePositiveDecimal(parts[index + 2], "distance"),
                        OfferCode = ValidateOfferCode(parts[index + 3]),
                        BaseDeliveryCost = baseCost
                    };
                    
                    packages.Add(package);
                }
                catch (Exception ex) when (ex is ArgumentException or FormatException)
                {
                    throw new ArgumentException(
                        $"Error parsing package {i + 1}: {ex.Message}", 
                        nameof(input), ex);
                }
            }

            // Parse vehicle information
            var vehicleIndex = 2 + (packageCount * 4);
            
            if (!int.TryParse(parts[vehicleIndex], out int vehicles))
                throw new ArgumentException(
                    $"Invalid vehicle count: '{parts[vehicleIndex]}'", 
                    nameof(input));
            
            if (vehicles <= 0)
                throw new ArgumentException(
                    "Vehicle count must be positive", 
                    nameof(input));

            if (!decimal.TryParse(parts[vehicleIndex + 1], out decimal maxSpeed))
                throw new ArgumentException(
                    $"Invalid max speed: '{parts[vehicleIndex + 1]}'", 
                    nameof(input));
            
            if (maxSpeed <= 0)
                throw new ArgumentException(
                    "Max speed must be positive", 
                    nameof(input));

            if (!decimal.TryParse(parts[vehicleIndex + 2], out decimal maxCarriableWeight))
                throw new ArgumentException(
                    $"Invalid max carriable weight: '{parts[vehicleIndex + 2]}'", 
                    nameof(input));
            
            if (maxCarriableWeight <= 0)
                throw new ArgumentException(
                    "Max carriable weight must be positive", 
                    nameof(input));

            return (packages, vehicles, maxSpeed, maxCarriableWeight);
        }

        #region Private Helper Methods

        private string ValidatePackageId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Package ID cannot be null or empty");
            
            return id.Trim();
        }

        private decimal ParsePositiveDecimal(string value, string fieldName)
        {
            if (!decimal.TryParse(value, out decimal result))
                throw new ArgumentException(
                    $"Invalid {fieldName} value: '{value}'");
            
            if (result <= 0)
                throw new ArgumentException(
                    $"{fieldName} must be positive");
            
            return result;
        }

        private string ValidateOfferCode(string offerCode)
        {
            if (string.IsNullOrWhiteSpace(offerCode))
                return null;
            
            return offerCode.Trim();
        }

        #endregion
    }
}