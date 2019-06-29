using MeetingScheduler.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.Models
{
    public class Meeting
    {
        [HiddenInput (DisplayValue=false)]
        public int Id { get; set; }
        public string Title { get; set; }
        [HiddenInput(DisplayValue = false)]
        //[System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan TimeStart { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime Start { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime End { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan TimeEnd { get; set; }
        public int ResourceId { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public string UserId { get; set; }
        //public User User { get; set; }
        public string UserIds { get; set; }
        //public List<EventsUsers> EventsUsers { get; set; }
        [HiddenInput(DisplayValue = false)]
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string[] SelectedPeople { get; set; }
       
        //{
        //    get { return this.Description.Replace(",", ";"); }
        //}

        //public Meeting()
        //{
        //    EventsUsers = new List<EventsUsers>();
        //}
    }
}
