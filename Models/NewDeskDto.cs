using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBookingSystem.Models
{
    public class NewDeskDto
    {
        public bool Available { get; set; } = true;
        public string LocationName { get; set; }
    }
}
