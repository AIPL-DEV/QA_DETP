using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;

namespace DETP
{
    public class SHAViewRequests : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("app_name").Equals("SHA") && HttpContext.Session.GetString("role_name").Equals("Admin") || HttpContext.Session.GetString("role_name").Equals("Super Admin") || HttpContext.Session.GetString("app_name").Equals("SHA"))
            {

                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "View SHA Requests";
                    DataTable my_request = null, requests_for_me = null;

                    sqlhelp.fetch1("select id,datetime, job_request_no, (select department_abbr from department where department_id = q.department) as department, name_of_structure,status from sha_request q where request_by = '" + HttpContext.Session.GetInt32("user") + "' order by id desc");
                    if (sqlhelp.datatable1.Rows.Count > 0)
                    {
                        my_request = sqlhelp.datatable1;
                    }
                    sqlhelp.fetch1("select id,datetime, job_request_no, (select department_abbr from department where department_id = q.department) as department, name_of_structure,status from sha_request q order by id desc");
                    if (sqlhelp.datatable1.Rows.Count > 0)
                    {
                        requests_for_me = sqlhelp.datatable1;
                    }
                    Tuple<DataTable, DataTable> values = new(my_request, requests_for_me);
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
