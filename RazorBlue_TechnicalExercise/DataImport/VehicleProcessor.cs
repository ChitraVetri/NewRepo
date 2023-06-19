using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Vehicle
{
    public string Registration { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public string Colour { get; set; }
    public string FuelType { get; set; }
}

public class VehicleProcessor
{
    public static void Main()
    {
        // Import the data from the CSV file
        List<Vehicle> vehicles = ImportDataFromCSV("C:\\Users\\vetri\\OneDrive\\Desktop\\chitravetri\\RazorBlue_TechnicalExercise\\DataImport\\Technical Test Data.csv");

        // Remove duplicates based on vehicle registration
        List<Vehicle> uniqueVehicles = vehicles.GroupBy(v => v.Registration)
                                               .Select(g => g.First())
                                               .ToList();

        // Export CSV files for each fuel type
        ExportCSVsByFuelType(uniqueVehicles);

        // Find vehicles with a valid registration
        List<Vehicle> validRegistrationVehicles = uniqueVehicles.Where(v => IsRegistrationValid(v.Registration))
                                                                .ToList();

        Console.WriteLine("Number of vehicles with a valid registration: " + validRegistrationVehicles.Count);

        // Count vehicles without a valid registration
        int invalidRegistrationCount = uniqueVehicles.Count - validRegistrationVehicles.Count;
        Console.WriteLine("Number of vehicles without a valid registration: " + invalidRegistrationCount);
        Console.ReadLine();
    }

    public static List<Vehicle> ImportDataFromCSV(string filePath)
    {
        List<Vehicle> vehicles = new List<Vehicle>();

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 4)
                    {
                        Vehicle vehicle = new Vehicle
                        {
                            Registration = parts[0],
                            Make = parts[1],
                            Model = parts[2],
                            Colour = parts[3],
                            FuelType = parts[4]
                        };
                        vehicles.Add(vehicle);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading the CSV file: " + ex.Message);
        }

        return vehicles;
    }

    public static void ExportCSVsByFuelType(List<Vehicle> vehicles)
    {
        var fuelTypes = vehicles.Select(v => v.FuelType).Distinct();

        foreach (string fuelType in fuelTypes)
        {
            List<Vehicle> filteredVehicles = vehicles.Where(v => v.FuelType == fuelType).ToList();
            string fileName = fuelType.ToLower() + ".csv";
            ExportDataToCSV(filteredVehicles, fileName);
        }
    }

    public static void ExportDataToCSV(List<Vehicle> vehicles, string filePath)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Vehicle vehicle in vehicles)
                {
                    writer.WriteLine($"{vehicle.Registration},{vehicle.Make},{vehicle.Model},{vehicle.FuelType}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error exporting data to CSV: " + ex.Message);
        }
    }

    public static bool IsRegistrationValid(string registration)
    {
        // Valid registration format: Two letters followed by two numbers, followed by a space and then three letters
        string pattern = @"^[A-Z]{2}\d{2}\s[A-Z]{3}$";
        return System.Text.RegularExpressions.Regex.IsMatch(registration, pattern);
    }
}
