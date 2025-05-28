using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DETP.Controllers
{
    public class JobController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.Get("user") != null)
            {
                ViewBag.Title = "Job";
                return View();
            }
            else
                return RedirectToAction("Login", "Account");
        }
    }
}
