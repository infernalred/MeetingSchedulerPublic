using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MeetingScheduler.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MeetingScheduler.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace MeetingScheduler.Controllers
{
    public class HomeController : Controller
    {
        private readonly MeetingContext _db1;
        private readonly MailService _mailService;
        
        public HomeController(MeetingContext context, MailService mailService)
        {
            _db1 = context;
            _mailService = mailService;  
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        //Json for fullcalendario
        public JsonResult GetRooms()
        {
            return Json(_db1.Rooms.ToList());
        }

        //Json for fullcalendario
        public JsonResult GetEvents()
        {
            return Json(_db1.Meeting.ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            Meeting meet = await _db1.Meeting.FirstOrDefaultAsync(m => m.Id == id);
            if (meet != null)
            {
                meet.SelectedPeople = meet.UserIds.Split(",").ToArray();
                meet.Room = _db1.Rooms.Find(meet.RoomId);
                //meet.SelectedPeople = meet.EventsUsers.Select(m => m.UserId).ToArray();
                return View(meet);
            }
            return NotFound();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            string username = User.Identity.Name;
            ViewData["UserId"] = username;
            ViewData["DropDownRoom"] = new SelectList(_db1.Rooms, "Id", "Title");
            ViewData["DropDownPeople"] = new MultiSelectList(_db1.Users, "Id", "CN");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Create(Meeting meet)
        {
            meet.Start = new DateTime(meet.Date.Year, meet.Date.Month, meet.Date.Day, meet.TimeStart.Hours, meet.TimeStart.Minutes, meet.TimeStart.Seconds);
            meet.End = new DateTime(meet.Date.Year, meet.Date.Month, meet.Date.Day, meet.TimeEnd.Hours, meet.TimeEnd.Minutes, meet.TimeEnd.Seconds);
            meet.ResourceId = meet.RoomId;
            meet.UserIds = string.Join(",", meet.SelectedPeople);
            meet.Description = meet.UserIds;
            _db1.Add(meet);
            //foreach (var user in meet.SelectedPeople)
            //{
            //    meet.EventsUsers.Add(new EventsUsers { MeetingId = meet.Id, UserId = user });
            //}
            
            await _db1.SaveChangesAsync();
            await Message1(meet, Models.Type.MeetCreated);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            Meeting meet = await _db1.Meeting.FirstOrDefaultAsync(m => m.Id == id);
            if (meet != null)
            {
                meet.SelectedPeople = meet.UserIds.Split(",").ToArray();
                //meet.SelectedPeople = meet.EventsUsers.Select(m => m.UserId).ToArray();
                string username = User.Identity.Name;
                ViewData["UserId"] = username;
                ViewData["DropDownRoom"] = new SelectList(_db1.Rooms, "Id", "Title");
                ViewData["DropDownPeople"] = new MultiSelectList(_db1.Users, "Id", "CN");
                return View(meet);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(Meeting meet)
        {
            //Meeting tmp = await _db1.Meeting.FirstOrDefaultAsync(m => m.Id == meet.Id);
            //DateTime Start = new DateTime(meet.Date.Year, meet.Date.Month, meet.Date.Day, meet.TimeStart.Hours, meet.TimeStart.Minutes, meet.TimeStart.Seconds);
            meet.Start = new DateTime(meet.Date.Year, meet.Date.Month, meet.Date.Day, meet.TimeStart.Hours, meet.TimeStart.Minutes, meet.TimeStart.Seconds);
            meet.End = new DateTime(meet.Date.Year, meet.Date.Month, meet.Date.Day, meet.TimeEnd.Hours, meet.TimeEnd.Minutes, meet.TimeEnd.Seconds);
            //tmp.Date = meet.Date;
            //tmp.RoomId = meet.RoomId;
            //tmp.TimeEnd = meet.TimeEnd;
            //tmp.TimeStart = meet.TimeStart;
            //tmp.Title = meet.Title;
            meet.ResourceId = meet.RoomId;
            meet.UserIds = string.Join(",", meet.SelectedPeople);
            meet.Description = meet.UserIds;
            //tmp.EventsUsers.Clear();
            _db1.Meeting.Update(meet);
            //if (meet.SelectedPeople != null)
            //{

            //    foreach (var user in meet.SelectedPeople)
            //    {
            //        tmp.EventsUsers.Add(new EventsUsers { MeetingId = meet.Id, UserId = user });
            //    }
            //}

            await _db1.SaveChangesAsync();

            await Message1(meet, Models.Type.MeetEdited);
            return RedirectToAction("Index");
        }

        public async Task Message1(Meeting meet, Enum type)
        {
            Message message = new Message();
            foreach (string user in meet.SelectedPeople)
            {
                string email = _db1.Users.Find(user).EmailAddress;
                message.Users.Add(email);
            }
            message.Room = _db1.Rooms.Find(meet.RoomId).Title;
            message.Theme = meet.Title;
            message.Body = _body[type] (meet.Start.ToString(), meet.TimeEnd.ToString(), message.Room);
            await _mailService.SendEmailAsync(message);
        }

        private Dictionary<Enum, Func<string, string, string, string>> _body = new Dictionary<Enum, Func<string, string, string, string>>
        {
            { Models.Type.MeetCreated, MeetCreated },
            { Models.Type.MeetEdited, MeetEdited },
            { Models.Type.MeetCanceled, MeetCanceled },
        };


        static string MeetCreated(string start, string end, string room)
        {
            return "The meeting has been created. From " + start + " to " + end + " in room " + room;
        }

        static string MeetEdited(string start, string end, string room)
        {
            return "The meeting has been changed. New time from " + start + " to " + end + " in room " + room;
        }

        static string MeetCanceled(string start, string end, string room)
        {
            return "The meeting has been canceled.";
        }
    }
    
}
