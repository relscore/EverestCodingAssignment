
using CourierService.Core.Models;
using CourierService.Core.Services;
using CourierService.Core.Services.Delivery;
using CourierService.Infrastructure.Parsers;
using CourierService.Infrastructure.Offers;
using Microsoft.Extensions.DependencyInjection;

namespace CourierService.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup dependency injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IOfferService, OfferService>()
                .AddSingleton<IDeliveryCostCalculator, DeliveryCostCalculator>()
                .AddSingleton<IDeliveryTimeCalculator, DeliveryTimeCalculator>()
                .AddSingleton<IInputParser, InputParser>()
                .AddSingleton<IPackageProcessor, PackageProcessor>()
                .BuildServiceProvider();

            Console.WriteLine("=== Kiki's Courier Service ===");
            Console.WriteLine("Select mode:");
            Console.WriteLine("1. Delivery Cost Estimation Only");
            Console.WriteLine("2. Delivery Time Estimation");
            Console.Write("Enter choice (1 or 2): ");
            
            var choice = Console.ReadLine();
            
            var parser = serviceProvider.GetService<IInputParser>();
            var packageProcessor = serviceProvider.GetService<IPackageProcessor>();
            
            if (choice == "1")
            {
                ProcessDeliveryCostOnly(parser, packageProcessor);
            }
            else if (choice == "2")
            {
                ProcessDeliveryWithTime(parser, packageProcessor);
            }
            else
            {
                Console.WriteLine("Invalid choice!");
            }
        }

        static void ProcessDeliveryCostOnly(IInputParser parser, IPackageProcessor processor)
        {
            Console.WriteLine("\n=== Delivery Cost Estimation ===");
            Console.WriteLine("Input format: base_delivery_cost no_of_packages pkg_id1 pkg_weight1_in_kg distance1_in_km offer_code1 ...");
            Console.WriteLine("Example: 100 3 PKG1 5 5 OFR001 PKG2 15 5 OFR002 PKG3 10 100 OFR003");
            Console.Write("\nEnter input: ");
            
            var input = Console.ReadLine();
            var packages = parser.ParseDeliveryCostInput(input);
            var results = processor.ProcessPackagesForDeliveryCost(packages);
            
            Console.WriteLine("\n=== Results ===");
            foreach (var result in results)
            {
                Console.WriteLine($"{result.PackageId} {result.Discount:0.##} {result.TotalCost:0.##}");
            }
        }

        static void ProcessDeliveryWithTime(IInputParser parser, IPackageProcessor processor)
        {
            Console.WriteLine("\n=== Delivery Time Estimation ===");
            Console.WriteLine("Input format: base_delivery_cost no_of_packages pkg_id1 pkg_weight1_in_kg distance1_in_km offer_code1 .... no_of_vehicles max_speed max_carriable_weight");
            Console.WriteLine("Example: 100 5 PKG1 50 30 OFR001 PKG2 75 125 OFFR08 PKG3 175 100 OFR003 PKG4 110 60 OFFR002 PKG5 155 95 NA 2 70 200");
            Console.Write("\nEnter input: ");
            
            var input = Console.ReadLine();
            var (packages, vehicles, maxSpeed, maxCarriableWeight) = parser.ParseDeliveryTimeInput(input);
            var results = processor.ProcessPackagesWithDeliveryTime(packages, vehicles, maxSpeed, maxCarriableWeight);
            
            Console.WriteLine("\n=== Results ===");
            foreach (var result in results)
            {
                Console.WriteLine($"{result.PackageId} {result.Discount:0.##} {result.TotalCost:0.##} {result.EstimatedDeliveryTime:0.##}");
            }
        }
    }
}
