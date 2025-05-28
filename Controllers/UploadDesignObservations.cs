using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace DETP.Controllers
{
    public class UploadDesignObservations : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("app_name").Equals("Design") && HttpContext.Session.GetString("role_name").Equals("Admin") || HttpContext.Session.GetString("role_name").Equals("Super Admin") || HttpContext.Session.GetString("app_name").Equals("Design"))
            {

                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "Upload Site Observation";
                    return View();
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            else
                return RedirectToAction("Login", "Account");
        }
        public JsonResult Submit(
          string job_name,
          string location,
          string ac,
          string date,
          List<string> observation,
          List<string> photos
       )
        {
            string job_request_no;
            string status="";
            try
            {
                String mm = DateTime.Now.ToString("MM");
                String yy = DateTime.Now.ToString("yyyy");
                if (ac.Equals("save"))
                {
                    status = "Pending";
                }
                else if (ac.Equals("fwd"))
                {
                    status = "Saved";
                }
                sqlhelp.fetch1("SELECT ISNULL(MAX(sl)+1, 1) as sl FROM design WHERE MONTH(datetime)='" + mm + "' AND YEAR(datetime)='" + yy + "'");
                String serial_no = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                job_request_no = "DESIGN/" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yy") + "/" + serial_no;
                sqlhelp.insert("design", serial_no, job_request_no, date, location, job_name,status, HttpContext.Session.GetInt32("user").ToString());
                int k = 0;

                sqlhelp.fetch1("SELECT ISNULL(MAX(id), 1) as id FROM design WHERE job_no='" + job_request_no + "' AND datetime='" + date + "' AND location='" + location + "' AND job_name='" + job_name + "' AND entered_by='" + HttpContext.Session.GetInt32("user").ToString() + "'");
                String serial_no1 = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();

                foreach (String img1 in photos)
                {
                    if (img1 != null)
                    {
                        sqlhelp.insert("design_photos", img1.ToString(), observation[k], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), serial_no1);
                        k++;
                    }
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "fail2" });
            }
            return Json(new { status = true, message = "success", job_request_no = job_request_no });
        }
    }
}
