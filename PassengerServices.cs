using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AirportTicketBookingSystem.Enums;

namespace AirportTicketBookingSystem
{
    public class PassengerServices
    {
        private List<Flight> flights = new List<Flight>();
        private List<Passenger> passengers = new List<Passenger>();
        private int bookingIdCounter = 1;
        string csvFilePath;
        public void AddFlight(Flight flight)
        {
            flight.FlightNumber = bookingIdCounter++;
            flights.Add(flight);

            // Update CSV file
            CsvHelper.WriteToCsv(csvFilePath, flights,
                                 f => $"{f.FlightNumber},{f.DepartureCountry},{f.DestinationCountry},{f.DepartureDate},{f.DepartureAirport},{f.ArrivalAirport},{f.Class},{f.Price}");
        }

        public List<Flight> SearchFlights(string departureCountry, string destinationCountry, DateTime departureDate, FlightClass flightClass)
        {
            // Read flights from CSV file
            List<Flight> flights = CsvHelper.ReadFromCsv(csvFilePath,
                values => new Flight
                {
                    FlightNumber = int.Parse(values[0]),
                    DepartureCountry = values[1],
                    DestinationCountry = values[2],
                    DepartureDate = DateTime.Parse(values[3]),
                    DepartureAirport = values[4],
                    ArrivalAirport = values[5],
                    Class = Enum.Parse<FlightClass>(values[6]),
                    Price = decimal.Parse(values[7]),
                    IsBooked = bool.Parse(values[8])
                });

            //search
            var filteredFlights = flights
                .Where(f =>
                    f.DepartureCountry == departureCountry &&
                    f.DestinationCountry == destinationCountry &&
                    f.DepartureDate.Date == departureDate.Date &&
                    f.Class == flightClass &&
                    !f.IsBooked
                )
                .ToList();

            return filteredFlights;
        }

        public void BookFlight(int flightNumber, string passengerName)
        {
            // Read flights and passengers from CSV
            List<Flight> flights = CsvHelper.ReadFromCsv(csvFilePath,
                values => new Flight
                {
                    FlightNumber = int.Parse(values[0]),
                    DepartureCountry = values[1],
                    DestinationCountry = values[2],
                    DepartureDate = DateTime.Parse(values[3]),
                    DepartureAirport = values[4],
                    ArrivalAirport = values[5],
                    Class = Enum.Parse<FlightClass>(values[6]),
                    Price = decimal.Parse(values[7]),
                    IsBooked = bool.Parse(values[8])
                });

            List<Passenger> passengers = CsvHelper.ReadFromCsv(csvFilePath,
                values => new Passenger
                {
                    Name = values[0],
                    BookingIds = values[1].Split(';').Select(int.Parse).ToList()
                });

            // Find the selected flight
            Flight selectedFlight = flights.FirstOrDefault(f => f.FlightNumber == flightNumber && !f.IsBooked);

            if (selectedFlight != null)
            {
                // Update flight booking status
                selectedFlight.IsBooked = true;

                // Find or create the passenger
                Passenger passenger = passengers.FirstOrDefault(p => p.Name == passengerName);

                if (passenger == null)
                {
                    passenger = new Passenger { Name = passengerName };
                    passengers.Add(passenger);
                }

                // Update passenger's booking information
                passenger.BookingIds.Add(selectedFlight.FlightNumber);

                Console.WriteLine($"Booking successful! Flight {selectedFlight.FlightNumber} booked for {passengerName}.");

                // Write updated flights and passengers back to CSV
                CsvHelper.WriteToCsv(csvFilePath, flights,
                                     f => $"{f.FlightNumber},{f.DepartureCountry},{f.DestinationCountry},{f.DepartureDate},{f.DepartureAirport},{f.ArrivalAirport},{f.Class},{f.Price},{f.IsBooked}");

                CsvHelper.WriteToCsv(csvFilePath, passengers,
                                     p => $"{p.Name},{string.Join(";", p.BookingIds)}");
            }
            else
            {
                Console.WriteLine("Invalid flight selection or the flight is already booked.");
            }
        }

        public void CancelBooking(int bookingId, string passengerName)
        {
            List<Flight> flights = CsvHelper.ReadFromCsv(csvFilePath,
                values => new Flight
                {
                    FlightNumber = int.Parse(values[0]),
                    DepartureCountry = values[1],
                    DestinationCountry = values[2],
                    DepartureDate = DateTime.Parse(values[3]),
                    DepartureAirport = values[4],
                    ArrivalAirport = values[5],
                    Class = Enum.Parse<FlightClass>(values[6]),
                    Price = decimal.Parse(values[7]),
                    IsBooked = bool.Parse(values[8])
                });

            List<Passenger> passengers = CsvHelper.ReadFromCsv(csvFilePath,
                values => new Passenger
                {
                    Name = values[0],
                    BookingIds = values[1].Split(';').Select(int.Parse).ToList()
                });

            // Find the passenger
            Passenger passenger = passengers.FirstOrDefault(p => p.Name == passengerName);

            if (passenger != null && passenger.BookingIds.Contains(bookingId))
            {
                // Find the booked flight
                Flight bookedFlight = flights.FirstOrDefault(f => f.FlightNumber == bookingId);

                if (bookedFlight != null)
                {
                    bookedFlight.IsBooked = false;

                    // Remove booking ID from passenger
                    passenger.BookingIds.Remove(bookingId);

                    Console.WriteLine($"Booking canceled! Flight {bookingId} canceled for {passengerName}.");

                    CsvHelper.WriteToCsv(csvFilePath, flights,
                                         f => $"{f.FlightNumber},{f.DepartureCountry},{f.DestinationCountry},{f.DepartureDate},{f.DepartureAirport},{f.ArrivalAirport},{f.Class},{f.Price},{f.IsBooked}");

                    CsvHelper.WriteToCsv(csvFilePath, passengers,
                                         p => $"{p.Name},{string.Join(";", p.BookingIds)}");
                }
                else
                {
                    Console.WriteLine($"Invalid booking ID: {bookingId}.");
                }
            }
            else
            {
                Console.WriteLine($"Passenger {passengerName} does not have a booking with ID {bookingId}.");
            }
        }

        public void ModifyBooking(int bookingId, string passengerName, Flight newFlight)
        {
            List<Flight> flights = CsvHelper.ReadFromCsv(csvFilePath,
                values => new Flight
                {
                    FlightNumber = int.Parse(values[0]),
                    DepartureCountry = values[1],
                    DestinationCountry = values[2],
                    DepartureDate = DateTime.Parse(values[3]),
                    DepartureAirport = values[4],
                    ArrivalAirport = values[5],
                    Class = Enum.Parse<FlightClass>(values[6]),
                    Price = decimal.Parse(values[7]),
                    IsBooked = bool.Parse(values[8])
                });

            List<Passenger> passengers = CsvHelper.ReadFromCsv(csvFilePath,
                values => new Passenger
                {
                    Name = values[0],
                    BookingIds = values[1].Split(';').Select(int.Parse).ToList()
                });

            // Cancel the existing booking
            CancelBooking(bookingId, passengerName);

            // Book the new flight
            BookFlight(newFlight.FlightNumber, passengerName);

            Console.WriteLine($"Booking modified! New flight {newFlight.FlightNumber} booked for {passengerName}.");

            CsvHelper.WriteToCsv(csvFilePath, flights,
                                 f => $"{f.FlightNumber},{f.DepartureCountry},{f.DestinationCountry},{f.DepartureDate},{f.DepartureAirport},{f.ArrivalAirport},{f.Class},{f.Price},{f.IsBooked}");

            CsvHelper.WriteToCsv(csvFilePath, passengers,
                                 p => $"{p.Name},{string.Join(";", p.BookingIds)}");
        }

        public List<Flight> ViewPersonalBookings(string passengerName)
        {
            List<Flight> flights = CsvHelper.ReadFromCsv(csvFilePath,
                values => new Flight
                {
                    FlightNumber = int.Parse(values[0]),
                    DepartureCountry = values[1],
                    DestinationCountry = values[2],
                    DepartureDate = DateTime.Parse(values[3]),
                    DepartureAirport = values[4],
                    ArrivalAirport = values[5],
                    Class = Enum.Parse<FlightClass>(values[6]),
                    Price = decimal.Parse(values[7]),
                    IsBooked = bool.Parse(values[8])
                });

            List<Passenger> passengers = CsvHelper.ReadFromCsv(csvFilePath,
                values => new Passenger
                {
                    Name = values[0],
                    BookingIds = values[1].Split(';').Select(int.Parse).ToList()
                });

            Passenger passenger = passengers.FirstOrDefault(p => p.Name == passengerName);

            if (passenger != null)
            {
                List<Flight> bookedFlights = flights.Where(f => passenger.BookingIds.Contains(f.FlightNumber)).ToList();
                Console.WriteLine($"List of flights booked by {passengerName}:");
                foreach (var flight in bookedFlights)
                {
                    Console.WriteLine($"Flight {flight.FlightNumber}: {flight.DepartureAirport} to {flight.ArrivalAirport}, Class: {flight.Class}, Price: {flight.Price}");
                }
                return bookedFlights;
            }
            else
            {
                Console.WriteLine($"Passenger {passengerName} does not exist or has no bookings.");
                return new List<Flight>();
            }
        }
    }
}
