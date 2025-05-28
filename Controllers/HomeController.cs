using DETP.data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace DETP
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.Get("user") != null)
            {
                ViewBag.Title = "Dashboard";

                sqlhelp.fetch1("select pno,force_reset from users where user_id='" + HttpContext.Session.GetString("logged_user") + "'");
                String changepwd = "no";
                if (sqlhelp.datatable1.Rows[0].ItemArray[1].ToString().Equals("Y"))
                {
                    changepwd = "yes";
                }
                ViewBag.cp = changepwd;

                sqlhelp.fetch1("select serial_no from qa_observation");
                ViewBag.nos_qa = sqlhelp.datatable1.Rows.Count;

                sqlhelp.fetch1("select serial_no from qa_observation q where serial_no in (select observation_id from qa_flow where to_id = " + HttpContext.Session.GetInt32("user") + " AND completed='N') ");
                ViewBag.nos_pqa = sqlhelp.datatable1.Rows.Count;

                sqlhelp.fetch1("select id from sha_request");
                ViewBag.nos_sha = sqlhelp.datatable1.Rows.Count;

                sqlhelp.fetch1("select id from sha_request WHERE status!='Pending for Requester Confirmation'");
                ViewBag.nos_psha = sqlhelp.datatable1.Rows.Count;

                sqlhelp.fetch1("select id from sha_request WHERE status='Pending for Requester Confirmation'");
                ViewBag.nos_csha = sqlhelp.datatable1.Rows.Count;

                sqlhelp.fetch1("select id from design");
                ViewBag.nos_design = sqlhelp.datatable1.Rows.Count;

                string[] allStatus = { "With Site Incharge","With Project Incharge", "With Dept HOD", "With Business Head", "With Assignee Section", "With Head DETP", "With QA Officer", "With EIC DETP", "With HoD QA & SHA" };
                Dictionary<string, int> withStatus = new Dictionary<string, int>();

                foreach (var item in allStatus)
                {
                    var value = _context.QAObservations.Where(x=> x.Status == item).Count();
                    //sqlhelp.fetch1($"SELECT status FROM qa_observation WHERE status='{item}'");
                    //var value = sqlhelp.datatable1.Rows.Count.ToString();
                    withStatus.Add(item, value);
                }

                ViewBag.withStatus = withStatus;

                return View();
            }
            else
                return RedirectToAction("Login","Account");
        }
        [HttpPost]
        public ActionResult ChangePassword(String old_pwd, String new_pwd)
        {
            old_pwd = md5.encryption(old_pwd.ToString());
            sqlhelp.fetch1("select pno from users where user_id = '" + HttpContext.Session.GetInt32("user") + "'");
            String pno = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
            sqlhelp.fetch1("select password from users where user_id = '" + HttpContext.Session.GetInt32("user") + "'");
            String pwd = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
            if (pwd.Equals(old_pwd) && !old_pwd.Equals(new_pwd))
            {
                new_pwd = md5.encryption(new_pwd.ToString());
                sqlhelp.performAction("UPDATE users SET password='" + new_pwd + "',force_reset='N' WHERE pno='" + pno + "'");
                return Json(new { status = true, message = "success" });

            }
            else {
                return Json(new { status = false, message = "fail" });

            }
        }

        IConfigurationRoot GetConfiguration2()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        public ActionResult DashboardData([FromQuery] List<long> divisionIds, [FromQuery] List<long> departmentIds)
        {
            var configuration = GetConfiguration2();
            long allId = configuration.GetValue<long>("AllId");

            string clause = "";
            
            if(divisionIds.Contains(allId))
            {
                var departments = _context.Department.Where(x=>x.DivisionId == allId).Select(x => x.Id).ToList();
                var departmentStr = string.Join(",", departments);
                clause += $"where department in ({string.Join(",", departmentStr)})";
            }
            else if (divisionIds.Count > 0 && departmentIds.Count > 0)
            {
                clause += $" where division_id in ({string.Join(",",divisionIds)}) and department in ({string.Join(",", departmentIds)})";
            }
            else if(departmentIds.Count > 0)
            {
                clause += $" where department in ({string.Join(",", departmentIds)})";
            }
            else if(divisionIds.Count > 0)
            {
                clause += $" where division_id in  ({string.Join(",", divisionIds)})";
            }

            sqlhelp.fetch1("select serial_no from qa_observation " + clause);
            var nos_qa = sqlhelp.datatable1.Rows.Count;

            sqlhelp.fetch1("select serial_no from qa_observation q where serial_no in (select observation_id from qa_flow where to_id = " + HttpContext.Session.GetInt32("user") + " AND completed='N') " + clause.Replace("where","and"));
            var nos_pqa = sqlhelp.datatable1.Rows.Count;

            sqlhelp.fetch1("select id from sha_request");
            var nos_sha = sqlhelp.datatable1.Rows.Count;

            sqlhelp.fetch1("select id from sha_request WHERE status!='Pending for Requester Confirmation'");
            var nos_psha = sqlhelp.datatable1.Rows.Count;

            sqlhelp.fetch1("select id from sha_request WHERE status='Pending for Requester Confirmation'");
            var nos_csha = sqlhelp.datatable1.Rows.Count;

            sqlhelp.fetch1("select id from design");
            var nos_design = sqlhelp.datatable1.Rows.Count;

            string[] allStatus = { "With Site Incharge", "With Project Incharge", "With Dept HOD", "With Business Head", "With Assignee Section", "With Head DETP", "With QA Officer", "With EIC DETP" };
            Dictionary<string, string> withStatus = new Dictionary<string, string>();

            foreach (var item in allStatus)
            {
                sqlhelp.fetch1($"SELECT status FROM qa_observation WHERE status='{item}' " + clause.Replace("where","and") );
                var value = sqlhelp.datatable1.Rows.Count.ToString();
                withStatus.Add(item, value);
            }

            var statusby = withStatus;

            return Json(new { nos_qa, nos_pqa, nos_sha, nos_psha, nos_csha, nos_design, statusby });
        }

        public ActionResult PermissionError()
        {
            return View();
        }
    }
}

