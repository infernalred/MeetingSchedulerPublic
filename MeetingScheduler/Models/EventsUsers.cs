using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.Models
{
    public class EventsUsers
    {
        public int MeetingId { get; set; }
        public Meeting Meeting { get; set; }


        public string UserId { get; set; }
        public User User { get; set; }
    }
}
