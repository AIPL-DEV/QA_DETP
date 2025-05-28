using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DETP.Controllers
{
    public class RequestController : Controller
    {
        sqlhelp s = new sqlhelp();
        public IActionResult Index()
        {
            if (HttpContext.Session.Get("user") != null)
            {
                ViewBag.Title = "Request Generation";
                return View();
            }
            else
                return RedirectToAction("Login", "Account");
        }

        public ActionResult Submit(
            string date,
            string department,
            string name_of_structure,
            string cost_center,
            string priority,
            string contact_person_name,
            string contact_person_email,
            string contact_person_phone,
            string location,
            string structure_type,
            string number_of_structure,
            string description)
        {
            return SubmitApi(date, department, name_of_structure, cost_center, priority, contact_person_name, contact_person_email, contact_person_phone, location, structure_type, number_of_structure, description);
        }

        [HttpPost]
        public JsonResult SubmitApi(
            string date,
            string department,
            string name_of_structure,
            string cost_center,
            string priority,
            string contact_person_name,
            string contact_person_email,
            string contact_person_phone,
            string location,
            string structure_type,
            string number_of_structure,
            string description)
        {
            string job_request_no, request_id;
            try
            {
                sqlhelp.insert("request", date, department, name_of_structure, cost_center, priority, contact_person_name, contact_person_email, contact_person_phone, location, structure_type, number_of_structure, description,"");
                sqlhelp.fetch1("Select max(request_id) from request");
                request_id = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                job_request_no = "SHA-" + department.Replace(' ', '_') + "-" + request_id + "-" + structure_type.Replace(' ', '_');
                sqlhelp.performAction("update request set job_request_no = '" + job_request_no + "' where request_id = '" + request_id + "'");
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "fail" });
            }
            return Json(new { status = true, message = "success", job_request_no = job_request_no });
        }
    }
}
