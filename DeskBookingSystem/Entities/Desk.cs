using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBookingSystem.Entities
{
    public class Desk
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public bool Available { get; set; }
      
        public virtual Location Location { get; set; }  
    }
}
