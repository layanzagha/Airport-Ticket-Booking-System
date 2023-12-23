using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AirportTicketBookingSystem.Enums;

namespace AirportTicketBookingSystem
{
    public class ManagerServices
    {
        string filePath;

        public void FilterBookings()
        {
            Console.WriteLine("Filter Bookings");

            Console.Write("Enter Flight Number (leave empty to skip): ");
            string stringFlightNumber = Console.ReadLine();
            int? flightNumber = string.IsNullOrWhiteSpace(stringFlightNumber) ? (int?)null : int.Parse(stringFlightNumber);

            Console.Write("Enter Price (leave empty to skip): ");
            string stringPrice = Console.ReadLine();
            decimal? price = string.IsNullOrWhiteSpace(stringPrice) ? (decimal?)null : decimal.Parse(stringPrice);

            Console.Write("Enter Departure Country (leave empty to skip): ");
            string departureCountry = Console.ReadLine();

            Console.Write("Enter Destination Country (leave empty to skip): ");
            string destinationCountry = Console.ReadLine();

            Console.Write("Enter Departure Date (leave empty to skip): ");
            string StringDepartureDate = Console.ReadLine();
            DateTime? departureDate = string.IsNullOrWhiteSpace(StringDepartureDate) ? (DateTime?)null : DateTime.Parse(StringDepartureDate);

            Console.Write("Enter Departure Airport (leave empty to skip): ");
            string departureAirport = Console.ReadLine();

            Console.Write("Enter Arrival Airport (leave empty to skip): ");
            string arrivalAirport = Console.ReadLine();

            Console.Write("Enter Passenger (leave empty to skip): ");
            string passengerName = Console.ReadLine();

            Console.Write("Enter Class (leave empty to skip): ");
            string flightClass = Console.ReadLine();

            FlightClass flightClass1 = FlightClassParser.Parse(flightClass);
            Console.WriteLine($"Selected Flight Class: {flightClass1}");

            // Read flights from CSV file
            List<Flight> flights = CsvHelper.ReadFromCsv("path_to_your_csv_file.csv", values => new Flight
            {
                FlightNumber = int.Parse(values[0]),
                DepartureCountry = values[1],
                DestinationCountry = values[2],
                DepartureDate = DateTime.Parse(values[3]),
                DepartureAirport = values[4],
                ArrivalAirport = values[5],
                Class = (FlightClass)Enum.Parse(typeof(FlightClass), values[6]),
                Price = decimal.Parse(values[7])
            });

            var filteredFlights = flights
                .Where(f =>
                    (!flightNumber.HasValue || f.FlightNumber == flightNumber.Value) &&
                    (!price.HasValue || f.Price == price.Value) &&
                    (string.IsNullOrWhiteSpace(departureCountry) || f.DepartureCountry.Equals(departureCountry, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(destinationCountry) || f.DestinationCountry.Equals(destinationCountry, StringComparison.OrdinalIgnoreCase)) &&
                    (!departureDate.HasValue || f.DepartureDate.Date == departureDate.Value.Date) &&
                    (string.IsNullOrWhiteSpace(departureAirport) || f.DepartureAirport.Equals(departureAirport, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(arrivalAirport) || f.ArrivalAirport.Equals(arrivalAirport, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(passengerName) || f.PassengersOfFlight.Any(p => p.Name.Equals(passengerName, StringComparison.OrdinalIgnoreCase))) &&
                    (string.IsNullOrWhiteSpace(flightClass) || f.Class == flightClass1))
                .ToList();

            // Display filtered bookings
            DisplayFlights(filteredFlights);
        }

        public void BatchFlightUpload(List<Flight> flights)
        {
            Console.WriteLine("Batch Flight Upload");

            Console.Write("Enter the path of the CSV file: ");
            string filePath = Console.ReadLine();

            try
            {
                // Read flights from CSV file
                List<Flight> newFlights = CsvHelper.ReadFromCsv(filePath, values => new Flight
                {
                    FlightNumber = int.Parse(values[0]),
                    DepartureCountry = values[1],
                    DestinationCountry = values[2],
                    DepartureDate = DateTime.Parse(values[3]),
                    DepartureAirport = values[4],
                    ArrivalAirport = values[5],
                    Class = (FlightClass)Enum.Parse(typeof(FlightClass), values[6]),
                    Price = decimal.Parse(values[7])
                });

                // Validate imported flight data
                List<string> validationErrors = ValidateFlightData(newFlights,filePath);

                if (validationErrors.Any())
                {
                    Console.WriteLine("Validation Errors:");
                    foreach (var error in validationErrors)
                    {
                        Console.WriteLine(error);
                    }
                }
                else
                {
                    flights.AddRange(newFlights);
                    Console.WriteLine("Flights successfully imported.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private List<string> ValidateFlightData(List<Flight> newFlights, string validationCsvPath)
        {
            List<string> validationErrors = new List<string>();

            List<ValidationDetails> validationDetailsList = CsvHelper.ReadFromCsv(validationCsvPath, values => new ValidationDetails
            {
                FieldName = values[0],
                Type = values[1],
                Constraints = values[2]
            });

            foreach (var flight in newFlights)
            {
                var validationDetails = validationDetailsList.FirstOrDefault(v => v.FieldName == "DepartureCountry");

                if (validationDetails != null)
                {
                    if (validationDetails.Type == "Free Text" && string.IsNullOrWhiteSpace(flight.DepartureCountry))
                    {
                        validationErrors.Add($"{validationDetails.FieldName} is required.");
                    }
                    else if (validationDetails.Type == "Date Time" && flight.DepartureDate < DateTime.Today)
                    {
                        validationErrors.Add($"{validationDetails.FieldName} must be today or in the future.");
                    }
                }
            }
            return validationErrors;
        }

        public void DynamicModelValidationDetails(string validationCsvPath)
        {
            Console.WriteLine("Dynamic Model Validation Details");

            List<ValidationDetails> validationDetailsList = CsvHelper.ReadFromCsv(validationCsvPath, values => new ValidationDetails
            {
                FieldName = values[0],
                Type = values[1],
                Constraints = values[2]
            });

            // Display validation details
            foreach (var validationDetails in validationDetailsList)
            {
                Console.WriteLine($"* {validationDetails.FieldName}:");
                Console.WriteLine($"  - Type: {validationDetails.Type}");
                Console.WriteLine($"  - Constraint: {validationDetails.Constraints}");
            }
        }

        private void DisplayFlights(List<Flight> flights)
        {
               flights = CsvHelper.ReadFromCsv(filePath, values => new Flight
            {
                FlightNumber = int.Parse(values[0]),
                DepartureCountry = values[1],
                DestinationCountry = values[2],
                DepartureDate = DateTime.Parse(values[3]),
                DepartureAirport = values[4],
                ArrivalAirport = values[5],
                Class = (FlightClass)Enum.Parse(typeof(FlightClass), values[6]),
                Price = decimal.Parse(values[7])
            });
            Console.WriteLine("Flight Details:");
            foreach (var flight in flights)
            {
                Console.WriteLine($"Flight Number: {flight.FlightNumber}");
                Console.WriteLine($"Departure Country: {flight.DepartureCountry}");
                Console.WriteLine($"Destination Country: {flight.DestinationCountry}");
                Console.WriteLine($"Departure Date: {flight.DepartureDate}");
                Console.WriteLine($"Departure Airport: {flight.DepartureAirport}");
                Console.WriteLine($"Arrival Airport: {flight.ArrivalAirport}");
                Console.WriteLine($"Class: {flight.Class}");
                Console.WriteLine($"Price: {flight.Price}");
                Console.WriteLine();
            }
        }
    }
}
