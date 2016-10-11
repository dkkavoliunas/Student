using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Student.Repository;
using Microsoft.EntityFrameworkCore;
using Student.Models;

namespace Student.Web.Controllers
{
    public class App : Controller
    {
        private readonly StudentContext _context;
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public App(StudentContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var delta = DayOfWeek.Monday - DateTime.UtcNow.DayOfWeek;
            var monday = DateTime.UtcNow.AddDays(delta).Date;
           


            var user = _context.Users
                .Include(x => x.Subscriptions)
                .ThenInclude(x => x.Subgroup)
                .ThenInclude(x => x.Lectures)
                .FirstOrDefault();


            var lectures = user.Subscriptions
                .Select(x => x.Subgroup)
                .SelectMany(x => x.Lectures);

            lectures = lectures.Where(l => l.StartTime > (monday - UnixEpoch).TotalSeconds && l.StartTime < (monday.AddDays(7) - UnixEpoch).TotalSeconds);


            return View();
        }

        private static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }
    }
}
