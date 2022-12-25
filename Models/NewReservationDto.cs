using DeskBookingSystem.Entities;

namespace DeskBookingSystem.Models
{
    public class NewReservationDto
    {
        public DateTime ReservationStart { get; set; }
        public DateTime ReservationEnd { get; set; }
        public int UserId { get; set; }
        public int DeskId { get; set; }
    }
}
