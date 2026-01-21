using CourierService.Core.Models;

namespace CourierService.Core.Services.Delivery
{
    public class DeliveryTimeCalculator : IDeliveryTimeCalculator
    {
        private readonly IDeliveryCostCalculator _costCalculator;
        private readonly IOfferService _offerService;

        public DeliveryTimeCalculator(IDeliveryCostCalculator costCalculator, IOfferService offerService)
        {
            _costCalculator = costCalculator;
            _offerService = offerService;
        }

        public List<DeliveryResult> CalculateDeliveryTimes(List<Package> packages, int vehicleCount, decimal maxSpeed, decimal maxCarriableWeight)
        {
            var results = packages.Select(p => _costCalculator.CalculateDeliveryCost(p)).ToList();
            
            // Initialize vehicle fleet
            var vehicles = Enumerable.Range(1, vehicleCount)
                .Select(i => new Vehicle { Id = i, AvailableAfter = 0 })
                .ToList();

            // Sort packages by weight descending, then by distance ascending 
            var remainingPackages = packages
                .OrderByDescending(p => p.Weight)
                .ThenBy(p => p.Distance)
                .ToList();

            var shipments = new List<Shipment>();
            var currentTime = 0m;
            var packageDeliveryTimes = new Dictionary<string, decimal>();

            while (remainingPackages.Any())
            {
                // Get available vehicles sorted by availability
                var availableVehicles = vehicles
                    .Where(v => v.AvailableAfter <= currentTime)
                    .OrderBy(v => v.AvailableAfter)
                    .ThenBy(v => v.Id)
                    .ToList();

                if (!availableVehicles.Any())
                {
                    // No vehicles available, advance time to next available vehicle
                    currentTime = vehicles.Min(v => v.AvailableAfter);
                    continue;
                }

                foreach (var vehicle in availableVehicles)
                {
                    if (!remainingPackages.Any()) break;

                    // Create optimal shipment for this vehicle
                    var shipment = CreateOptimalShipment(remainingPackages, maxCarriableWeight);
                    
                    if (shipment != null && shipment.Packages.Any())
                    {
                        shipment.VehicleId = vehicle.Id;
                        shipment.DepartureTime = currentTime;
                        
                        // Sort packages by distance ASCENDING for delivery order (closest first)
                        shipment.Packages = shipment.Packages
                            .OrderBy(p => p.Distance)
                            .ToList();
                        
                        // Calculate cumulative delivery times for each package
                        decimal cumulativeTime = currentTime;
                        decimal previousDistance = 0;
                        
                        foreach (var pkg in shipment.Packages)
                        {
                            // Travel time from previous location to this package's destination
                            decimal travelSegment = (pkg.Distance - previousDistance) / maxSpeed;
                            cumulativeTime += travelSegment;
                            
                            // Record individual package delivery time
                            packageDeliveryTimes[pkg.Id] = Math.Round(cumulativeTime, 2);
                            previousDistance = pkg.Distance;
                        }
                        
                        // Total travel time is to farthest destination and back
                        shipment.TravelTime = shipment.Packages.Max(p => p.Distance) / maxSpeed;
                        shipment.ReturnTime = currentTime + (2 * shipment.TravelTime);
                        
                        shipments.Add(shipment);
                        
                        // Update vehicle availability
                        vehicle.AvailableAfter = shipment.ReturnTime;
                        
                        // Remove shipped packages from remaining list
                        foreach (var pkg in shipment.Packages)
                        {
                            remainingPackages.RemoveAll(p => p.Id == pkg.Id);
                        }
                    }
                    else
                    {
                        // No valid shipment can be created with remaining packages
                        break;
                    }
                }
                
                // If we still have packages but no vehicles available, advance time
                if (remainingPackages.Any())
                {
                    currentTime = vehicles.Min(v => v.AvailableAfter);
                }
            }

            // Update delivery times in results
            foreach (var result in results)
            {
                if (packageDeliveryTimes.TryGetValue(result.PackageId, out decimal deliveryTime))
                {
                    result.EstimatedDeliveryTime = deliveryTime;
                }
            }

            return results.ToList();
        }

        private Shipment CreateOptimalShipment(List<Package> packages, decimal maxWeight)
        {
            if (!packages.Any()) return null;

            var bestShipment = new Shipment();
            var bestWeight = 0m;
            var bestPackageCount = 0;
            var bestMaxDistance = decimal.MaxValue;

            // find the best combination following criteria:
            // 1. Shipment should contain max packages vehicle can carry
            // 2. Prefer heavier packages when same number of packages
            // 3. If weights are same, prefer shipment that can be delivered first (shorter max distance)
            
            // Start with maximum possible packages and work down
            for (int count = Math.Min(4, packages.Count); count >= 1; count--)
            {
                var combinations = GenerateCombinations(packages, count);
                
                foreach (var combo in combinations)
                {
                    var totalWeight = combo.Sum(p => p.Weight);
                    
                    if (totalWeight <= maxWeight)
                    {
                        var maxDistance = combo.Max(p => p.Distance);
                        
                        // Check if this combination is better than current best
                        bool isBetter = false;
                        
                        if (combo.Count > bestPackageCount)
                        {
                            isBetter = true;
                        }
                        else if (combo.Count == bestPackageCount && totalWeight > bestWeight)
                        {
                            isBetter = true;
                        }
                        else if (combo.Count == bestPackageCount && totalWeight == bestWeight && maxDistance < bestMaxDistance)
                        {
                            isBetter = true;
                        }
                        
                        if (isBetter)
                        {
                            bestPackageCount = combo.Count;
                            bestWeight = totalWeight;
                            bestMaxDistance = maxDistance;
                            bestShipment.Packages = combo.ToList();
                        }
                    }
                }
                
                // If we found a valid combination with current count, use it
                // Shipment should contain max packages vehicle can carry
                if (bestShipment.Packages.Any())
                {
                    break;
                }
            }

            return bestShipment.Packages.Any() ? bestShipment : null;
        }

        private List<List<Package>> GenerateCombinations(List<Package> packages, int count)
        {
            var result = new List<List<Package>>();
            
            if (count == 0)
            {
                result.Add(new List<Package>());
                return result;
            }
            
            if (!packages.Any())
                return result;
            
            var first = packages[0];
            var rest = packages.Skip(1).ToList();
            
            // Combinations including first element
            var combinationsWithFirst = GenerateCombinations(rest, count - 1);
            foreach (var combo in combinationsWithFirst)
            {
                combo.Insert(0, first);
                result.Add(combo);
            }
            
            // Combinations excluding first element
            var combinationsWithoutFirst = GenerateCombinations(rest, count);
            result.AddRange(combinationsWithoutFirst);
            
            return result;
        }
    }

    public class Shipment
    {
        public int VehicleId { get; set; }
        public List<Package> Packages { get; set; } = new List<Package>();
        public decimal DepartureTime { get; set; }
        public decimal TravelTime { get; set; }
        public decimal ReturnTime { get; set; }
        public bool IsDelivered => ReturnTime > 0;
    }
}