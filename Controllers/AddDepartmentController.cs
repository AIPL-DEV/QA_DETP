using DETP.data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DETP.Controllers
{
    public class AddDepartmentController : Controller
    {
        private ILogger<AddDepartmentController> _logger;
        private ApplicationDbContext _context;
        public AddDepartmentController(ILogger<AddDepartmentController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("role_name").Equals("Admin") || HttpContext.Session.GetString("role_name").Equals("Super Admin"))
            {
                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "Add Department";
                    ViewBag.divisions = _context.Divisions.ToList();
                    return View();
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            else
                return RedirectToAction("Login", "Account");
        }
       
        public ActionResult Submit(
            String department_abbr,
            String department_name,
            long division_id)
        {
            return SubmitApi(department_abbr, department_name,division_id);
        }

        [HttpPost]
        public JsonResult SubmitApi(
            String department_abbr,
            String department_name,
            long division_id)
        {
            try
            {
                _context.Department.Add(new model.Department
                {
                    DivisionId = division_id,
                    Abbr = department_abbr,
                    Name = department_name,
                    UpdatedOn = DateTime.Now
                });
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
                return Json(new { status = false, message = "fail" });
            }
            return Json(new { status = true, message = "success" });
        }
    }
}
