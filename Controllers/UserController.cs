using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DETP.auth;

namespace DETP.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("role_name").Equals("Admin") || HttpContext.Session.GetString("role_name").Equals("Super Admin"))
            {
                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "User";
                    if (HttpContext.Session.GetString("role_name").Equals("Super Admin"))
                    {
                        sqlhelp.fetch1("Select user_id,pno,name,email, (Select role from role where role.role_id = (select role_id from user_role where user_role.user_id = users.user_id )) as Role, (Select department_abbr from department where department_id = users.department) as Department,app from users WHERE pno!='"+ HttpContext.Session.GetString("pno") + "'");
                    }
                    else {
                        sqlhelp.fetch1("Select user_id,pno,name,email, (Select role from role where role.role_id = (select role_id from user_role where user_role.user_id = users.user_id )) as Role, (Select department_abbr from department where department_id = users.department) as Department,app from users WHERE app='" + HttpContext.Session.GetString("app_name") + "' AND pno!='" + HttpContext.Session.GetString("pno") + "'");
                    }
                    var user = sqlhelp.datatable1;

                    sqlhelp.fetch1("select department_id, department_abbr from department");
                    var department = sqlhelp.datatable1;

                    sqlhelp.fetch1("select role_id, role from role ORDER BY role asc");
                    var role = sqlhelp.datatable1;

                    Tuple<DataTable, DataTable, DataTable, Tuple<DataTable>> values = new Tuple<DataTable, DataTable, DataTable, Tuple<DataTable>>(user, department, role, new Tuple<DataTable>(user));
                    return View(values);

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
                sqlhelp.fetch1("Select id from sha_request WHERE request_by='" + user_id + "'");
                var shr = sqlhelp.datatable1.Rows.Count;
                sqlhelp.fetch1("Select id from sha_request WHERE sha_team_decision_by='" + user_id + "'");
                var shr2 = sqlhelp.datatable1.Rows.Count;
                sqlhelp.fetch1("Select id from sha_request WHERE visit_by='" + user_id + "'");
                var shr3 = sqlhelp.datatable1.Rows.Count;
                sqlhelp.fetch1("Select id from sha_request WHERE plan_test_date_entry_by='" + user_id + "'");
                var shr4 = sqlhelp.datatable1.Rows.Count;
                sqlhelp.fetch1("Select id from sha_request WHERE job_completed_entry_by='" + user_id + "'");
                var shr5 = sqlhelp.datatable1.Rows.Count;
                sqlhelp.fetch1("Select serial_no from qa_observation WHERE observation_by='" + user_id + "'");
                var qar = sqlhelp.datatable1.Rows.Count;
                sqlhelp.fetch1("Select id from qa_flow WHERE to_id='" + user_id + "'");
                var flow1 = sqlhelp.datatable1.Rows.Count;
                sqlhelp.fetch1("Select id from qa_flow WHERE from_id='" + user_id + "'");
                var flow2 = sqlhelp.datatable1.Rows.Count;
                if (shr == 0 && shr2 == 0 && shr3 == 0 && shr4 == 0 && shr5 == 0 && qar == 0 && flow1 == 0 && flow2 == 0)
                {
                    sqlhelp.performAction("Delete from users where user_id = '" + user_id + "'");
                    sqlhelp.performAction("Delete from user_role where user_id = '" + user_id + "'");
                }
                else {
                    return Json(new { status = false, message = "User already assigned with Observations/Requests" });

                }
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "fail" });
            }
            return Json(new { status = true, message = "success" });
        }
        public ActionResult ResetPwd(String user_id)
        {
            sqlhelp.fetch1("select email,name,pno from users where user_id = '" + user_id + "'");
            String pno = sqlhelp.datatable1.Rows[0].ItemArray[2].ToString();
            Random r = new Random();
            int n = r.Next();
            var password = md5.encryption(n.ToString());
            sqlhelp.performAction("UPDATE users SET password='" + password + "',force_reset='Y' where pno = '" + pno + "'");
            String email_id = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
            String name = sqlhelp.datatable1.Rows[0].ItemArray[1].ToString();
            String body="Dear <i>"+name+"</i>,<br><br>Please find the new reset password :-<b>"+n+ "</b><br><br><u>Note :- We request you to kindly copy & paste the password.</u><br><br>Thanks,<br>QA- DETP<br>Tata Steel UISL";
            mail.SendMail(email_id, "Reset Password of QA Application", body);
            return Json(new { status = true, message = "success" });
        }
        public ActionResult Update(String user_id)
        {

            try
            {
                sqlhelp.fetch2("select user_id,pno,name,email,department,(select role_id from user_role where user_role.user_id = users.user_id) as Role from users where user_id = '" + user_id + "'");
                DataTable data2 = sqlhelp.datatable2;
                return Json(new { id = sqlhelp.datatable2.Rows[0].ItemArray[0].ToString(),pno = sqlhelp.datatable2.Rows[0].ItemArray[1].ToString(), name = sqlhelp.datatable2.Rows[0].ItemArray[2].ToString(), email = sqlhelp.datatable2.Rows[0].ItemArray[3].ToString(), dept = sqlhelp.datatable2.Rows[0].ItemArray[4].ToString(), role = sqlhelp.datatable2.Rows[0].ItemArray[5].ToString() });

            }
            catch (Exception)
            {
                return Json(new { status = false, message = "fail" });
            }
        }
        [HttpPost]
        public ActionResult DoUpdate(String id, String pno, String name, String email, String dept, String role)
        {
            sqlhelp.performAction("UPDATE users SET pno='" + pno + "',name='" + name + "',email='" + email + "',department='" + dept + "',updated_on='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',updated_by='"+ HttpContext.Session.GetInt32("user") + "' WHERE user_id = '" + id + "'");
            sqlhelp.performAction("UPDATE user_role SET role_id='" + role + "' WHERE user_id = '" + id + "'");

            return Json(new { status = true, message = "success" });

        }
    }
}
