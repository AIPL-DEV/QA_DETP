using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Extensions.Logging;

namespace DETP.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ILogger<DepartmentController> _logger;
        public DepartmentController(ILogger<DepartmentController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("role_name").Equals("Admin") || HttpContext.Session.GetString("role_name").Equals("Super Admin"))
            {
                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "Department";
                    sqlhelp.fetch1("Select department_id,department_abbr, department_name from department");
                    return View(sqlhelp.datatable1);
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            else
                return RedirectToAction("Login", "Account");
        }
        public ActionResult Delete(String user_id)
        {
            try
            {
                sqlhelp.fetch1("Select id from sha_request WHERE department='"+user_id+"'");
                var shr = sqlhelp.datatable1.Rows.Count;
                sqlhelp.fetch1("Select serial_no from qa_observation WHERE department='" + user_id + "'");
                var qar = sqlhelp.datatable1.Rows.Count;
                if (shr == 0 && qar == 0)
                {
                    sqlhelp.performAction("Delete from department where department_id = '" + user_id + "'");
                }
                else {
                    return Json(new { status = false, message = "Department already assigned with Observations/Requests." });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
                return Json(new { status = false, message = "fail" });
            }
            return Json(new { status = true, message = "success" });
        }
        public ActionResult Update(String user_id)
        {

            try
            {
                sqlhelp.fetch2("select department_id,department_abbr,department_name from department where department_id = '" + user_id + "'");
                DataTable data2 = sqlhelp.datatable2;

                return Json(new { department_id = sqlhelp.datatable2.Rows[0].ItemArray[0].ToString(), department_abbr = sqlhelp.datatable2.Rows[0].ItemArray[1].ToString(), department_name = sqlhelp.datatable2.Rows[0].ItemArray[2].ToString() });

            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
                return Json(new { status = false, message = "fail" });
            }
        }
        [HttpPost]
        public ActionResult DoUpdate(String abb,String name,String id)
        {

            try
            {
                sqlhelp.performAction("UPDATE department SET department_abbr='" + abb + "',department_name='" + name + "',updated_on='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE department_id = '" + id + "'");

                return Json(new { status = true, message = "success" });

            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
                return Json(new { status = false, message = "fail" });
            }
        }
    }
}
