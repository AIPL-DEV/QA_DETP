using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;

namespace DETP
{
    public class ViewDesignObservations : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("app_name").Equals("Design") && HttpContext.Session.GetString("role_name").Equals("Admin") || HttpContext.Session.GetString("role_name").Equals("Super Admin") || HttpContext.Session.GetString("app_name").Equals("Design"))
            {

                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "Download Site Observations";
                    DataTable my_request = null;

                    sqlhelp.fetch1("select id,datetime, job_no, job_name, location,(select name from users where user_id = q.entered_by) as entered_by,status from design q order by id desc");
                    if (sqlhelp.datatable1.Rows.Count > 0)
                    {
                        my_request = sqlhelp.datatable1;
                    }

                    Tuple<DataTable> values = new(my_request);
                    return View(values);
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            else
                return RedirectToAction("Login", "Account");
        }
    }
}
