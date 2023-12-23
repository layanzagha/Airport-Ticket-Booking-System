using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingSystem
{
    public class Passenger
    {
        public string Name { get; set; }
        public List<int> BookingIds { get; set; } = new List<int>();
    }
}
