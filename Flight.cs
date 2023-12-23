using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AirportTicketBookingSystem.Enums;

namespace AirportTicketBookingSystem
{
    public class Flight
    {
        public int FlightNumber { get; set; }
        public string DepartureCountry { get; set; }
        public string DestinationCountry { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public FlightClass Class { get; set; }
        public decimal Price { get; set; }
        public bool IsBooked { get; set; }
        public List<Passenger> PassengersOfFlight { get; set; } = new List<Passenger>();

    }
}
