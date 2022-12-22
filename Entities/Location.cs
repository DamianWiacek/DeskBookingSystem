using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBookingSystem.Entities
{
    internal class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Desk> Desks { get; set; }
        public virtual List<Reservation>? Reservations { get; set; }
    }
}
