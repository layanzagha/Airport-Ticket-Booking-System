using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingSystem
{
    public class Enums
    {
        public enum FlightClass
        {
            Economy,
            Business,
            FirstClass
        }

        public static class FlightClassParser
        {
            public static FlightClass Parse(string value)
            {
                if (Enum.TryParse(value, ignoreCase: true, out FlightClass result))
                {
                    return result;
                }

                throw new ArgumentException("Invalid value for FlightClass");
            }
        }
    }
}
