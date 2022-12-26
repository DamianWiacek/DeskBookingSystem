using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBookingSystem.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime ReservationStart { get; set; }
        public DateTime ReservationEnd{ get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int DeskId { get; set; }
        public virtual Desk Desk { get; set; }
    }
}
