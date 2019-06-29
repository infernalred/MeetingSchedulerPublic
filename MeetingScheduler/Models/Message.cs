using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.Models
{
    public enum Type
    {
        MeetCreated,
        MeetEdited,
        MeetCanceled
    }
    public class Message
    {
        public string Theme { get; set; }
        public string Creator { get; set; }
        public List<string> Users { get; set; }
        public string Room { get; set; }
        public string Body { get; set; }
        public DateTime Start { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public Type Type { get; set; }

        public Message()
        {
            Users = new List<string>();
        }
    }
    
}
