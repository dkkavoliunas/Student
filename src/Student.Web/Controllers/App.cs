using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Student.Web.Controllers
{
    public class App : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
