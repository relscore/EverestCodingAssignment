# Kiki's Courier Service

A .NET console application for calculating delivery costs and estimated delivery times for a courier service.

## Features
1. **Delivery Cost Calculation**: Calculates delivery costs with applicable discounts based on offer codes
2. **Delivery Time Estimation**: Estimates delivery times with optimal vehicle scheduling
3. **Extensible Architecture**: Easy to add new offer codes and extend functionality

## Requirements
- .NET 8.0 SDK

## How to Run

### Mode 1: Delivery Cost Only
Input format: `base_delivery_cost no_of_packages pkg_id1 pkg_weight1_in_kg distance1_in_km offer_code1 ...`

Example: `100 3 PKG1 5 5 OFR001 PKG2 15 5 OFR002 PKG3 10 100 OFR003`

### Mode 2: Delivery Time Estimation
Input format: `base_delivery_cost no_of_packages pkg_id1 pkg_weight1_in_kg distance1_in_km offer_code1 .... no_of_vehicles max_speed max_carriable_weight`

Example: `100 5 PKG1 50 30 OFR001 PKG2 75 125 OFFR08 PKG3 175 100 OFR003 2 70 200`


CourierService/
├── Core/
│ ├── Models/ # Domain models
│ └── Services/ # Core business logic
├── Infrastructure/ # Infrastructure services
├── Program.cs # Main application
└── CourierService.ConsoleApp.csproj
