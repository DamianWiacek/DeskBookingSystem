using DeskBookingSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBookingSystem.Models
{
    public class DeskDtoForAdmin
    {
        public int Id { get; set; }
        public string LocationName { get; set; }
        public bool Available { get; set; }
        public int? ReservingUserId { get; set; }
    }
}
