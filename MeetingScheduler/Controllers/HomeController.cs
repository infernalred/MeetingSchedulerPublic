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

namespace MeetingScheduler.Controllers
{
    public class HomeController : Controller
    {
        private readonly MeetingContext _db1;
        private readonly PeopleService _people;
        private readonly MailService _mailService;
        public HomeController(MeetingContext context, PeopleService people, MailService mailService)
        {
            _people = people;
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
        public IActionResult Create()
        {
            ViewData["DropDownRoom"] = new SelectList(_db1.Rooms, "Id", "Title");
            ViewData["DropDownPeople"] = new MultiSelectList(_people.GetAllUser(), "Id", "CN");
            return View();
        }

        [HttpPost]
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
            await SendMessage(meet.SelectedPeople, meet.Title, "Вы были приглашены на событие.", meet.RoomId, meet.Start, meet.TimeEnd);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Meeting meet = await _db1.Meeting.FirstOrDefaultAsync(m => m.Id == id);
            if (meet != null)
            {
                meet.SelectedPeople = meet.UserIds.Split(",").ToArray();
                //meet.SelectedPeople = meet.EventsUsers.Select(m => m.UserId).ToArray();
                ViewData["DropDownRoom"] = new SelectList(_db1.Rooms, "Id", "Title");
                ViewData["DropDownPeople"] = new MultiSelectList(_people.GetAllUser(), "Id", "CN");
                return View(meet);
            }
            return NotFound();
        }

        [HttpPost]
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
            await SendMessage(meet.SelectedPeople, meet.Title, "Сообщаем об изменении события.", meet.RoomId, meet.Start, meet.TimeEnd);
            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public IActionResult GetPeoples(string search)
        //{
        //    var users = from s in _db1.Users select s;
        //    return Json(users = users.Where(s => s.CN.Contains(search)));
        //}

        public async Task SendMessage(string[] users, string theme, string body, int roomId, DateTime data, TimeSpan endTime)
        {

            string[] emails = new string[users.Length];
            for (int i = 0; i < users.Length; i++)
            {
                emails[i] = _people.allPeople[(users[i])].EmailAddress;
            }
            string room = _db1.Rooms.Find(roomId).Title;
            theme = theme + ".  " + body;
            string bodyMessage = "Митинг состоится " + data.ToString() + " до " + endTime.ToString() + " в переговорке " + room;
            await _mailService.SendEmailAsync(emails, theme, bodyMessage);
        }
    }
}
