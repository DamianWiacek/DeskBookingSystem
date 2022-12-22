using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBookingSystem.Entities
{
    internal class Reservation
    {
        public int Id { get; set; }
        public DateTime ReservationDate { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }
    }
}
