using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DETP.Controllers
{
    public class AddUserController : Controller
    {
        private readonly ILogger<AddUserController> _logger;
        public AddUserController(ILogger<AddUserController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("role_name").Equals("Admin") || HttpContext.Session.GetString("role_name").Equals("Super Admin"))
            {
                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "Add User";

                    sqlhelp.fetch1("Select department_id, department_abbr from department");
                    sqlhelp.fetch2("Select role_id, role from role");

                    Tuple<DataTable, DataTable> values = new Tuple<DataTable, DataTable>(sqlhelp.datatable1, sqlhelp.datatable2);
                    return View(values);
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            else
                return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public ActionResult Submit(
            String full_name,
            String personal_number,
            String email,
            String department,
            String app,
            String role)
        {
            return SubmitApi(full_name, personal_number, email, department, app, role);
        }

        [HttpPost]
        public JsonResult SubmitApi(
            String full_name,
            String personal_number,
            String email,
            String department,
            String app,
            String role)
        {
            try
            {
                int n = 0;
                String password = "";
                String force_reset = "";
                String mailk = "yes";
                sqlhelp.fetch1("SELECT user_id FROM users WHERE pno='" + personal_number + "' AND app='" + app + "'");
                if (sqlhelp.datatable1.Rows.Count.ToString().Equals("0"))
                {
                    sqlhelp.fetch1("SELECT user_id,password,force_reset FROM users WHERE pno='" + personal_number + "'");
                    if (!sqlhelp.datatable1.Rows.Count.ToString().Equals("0"))
                    {
                        password = sqlhelp.datatable1.Rows[0].ItemArray[1].ToString();
                        force_reset = sqlhelp.datatable1.Rows[0].ItemArray[2].ToString();
                        mailk = "no";
                    }
                    else
                    {
                        // Random r = new Random();
                        //n = r.Next();
                        n = 123456;
                        password = md5.encryption(n.ToString());
                        force_reset = "Y";

                    }
                    sqlhelp.insert("users", personal_number, password, full_name, email, department, app, "", "", force_reset, "N/A");

                    sqlhelp.fetch1("Select max(user_id) from users");
                    sqlhelp.insert("user_role", sqlhelp.datatable1.Rows[0].ItemArray[0].ToString(), role);

                    if (mailk.Equals("yes"))
                    {
                        String body = "Dear <i>" + full_name + "</i>,<br><br>Please find the password :-<b>" + n + "</b><br><br><u>Note :- We request you to kindly copy & paste the password.</u><br><br>Thanks,<br>QA- DETP<br>Tata Steel UISL";
                        mail.SendMail(email, "Password for " + app + " Application", body);
                    }
                }
                else
                {
                    return Json(new { status = false, message = "Duplicate ID" });

                }
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
