using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DETP.Controllers
{
    public class EditDesignRequest : Controller
    {
        

        public IActionResult Index(String serial_no)
        {
            if (HttpContext.Session.GetString("app_name").Equals("Design") && HttpContext.Session.GetString("role_name").Equals("Admin") || HttpContext.Session.GetString("role_name").Equals("Super Admin") || HttpContext.Session.GetString("app_name").Equals("Design"))
            {

                if (HttpContext.Session.Get("user") != null)
                {
                    DataTable my_request = null;
                    DataTable photos = null;

                    sqlhelp.fetch1("select id,datetime, job_no, job_name, location,(select name from users where user_id = q.entered_by) as entered_by,status from design q WHERE id='"+ serial_no + "'");
                    ViewBag.Title = sqlhelp.datatable1.Rows[0].ItemArray[2].ToString();
                      if (sqlhelp.datatable1.Rows.Count > 0)
                    {
                        my_request = sqlhelp.datatable1;
                    }
                    sqlhelp.fetch1("select * from design_photos q WHERE design_id='" + serial_no + "'");
                    if (sqlhelp.datatable1.Rows.Count > 0)
                    {
                        photos = sqlhelp.datatable1;
                    }
                    Tuple<DataTable, DataTable> values = new(my_request, photos);

                    return View(values);
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            else
                return RedirectToAction("Login", "Account");
        }
        public ActionResult Update(List<string> observation, List<string> photos, String request_id, String ac)
        {
            String status1 = "";
       
                if (ac.Equals("fwd"))
                {
                    status1 = "Saved";
                }
                else
                {
                    status1 = "Pending";

                }
                sqlhelp.performAction("UPDATE design SET status='" + status1 + "' WHERE id='" + request_id + "'");
                int k = 0;

                foreach (String img1 in photos)
                {
                    if (img1 != null)
                    {
                        sqlhelp.insert("design_photos", img1.ToString(), observation[k], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request_id);
                        k++;
                    }
                }
            return Json(new { status = true, message = "success" });

        }
        public ActionResult DeletePhoto(String id, String observation_id)
        {
            sqlhelp.performAction("DELETE FROM design_photos WHERE id='" + id + "'");
            return RedirectToAction("Index", "EditDesignRequest", new { serial_no = observation_id });
        }
    }
}
