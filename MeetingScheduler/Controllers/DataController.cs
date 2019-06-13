using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetingScheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.Controllers
{
    public class DataController : Controller
    {
        private readonly MeetingContext _db1;
        public DataController(MeetingContext context)
        {
            _db1 = context;
        }
        //public JsonResult GetPeoples()
        //{
        //    return Json(_db1.Users.ToList());
        //}
    }
}