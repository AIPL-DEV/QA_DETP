using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

namespace DETP.Controllers
{
    public class SHAReport : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("app_name").Equals("SHA") && HttpContext.Session.GetString("role_name").Equals("SHA Team") || HttpContext.Session.GetString("role_name").Equals("Super Admin"))
            {

                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "SHA Request Report";
                    DataTable my_request = null;

                    sqlhelp.fetch1("select id,datetime, job_request_no, (select department_abbr from department where department_id = q.department) as department, name_of_structure,cost_center,priority,contact_person_name,contact_person_email,contact_person_phone,location,structure_type,number_of_structure,description,sha_team_decision,(select name from users where user_id = q.sha_team_decision_by) AS sha_team_decision_by,sha_team_decision_datetime,remarks,planned_visit_date,(select name from users where user_id = q.request_by) AS request_by,actual_visit_datetime,(select name from users where user_id = q.visit_by) AS visit_by,rcompleted_datetime as VIR_Completed,rconfirmation_datetime AS customer_confirmation_date,arrdes AS arrangement_description,plan_test_date,(select name from users where user_id = q.plan_test_date_entry_by) AS plan_test_date_entry_by,plan_test_date_entry_datetime,actual_test_datetime,job_completed_datetime,(select name from users where user_id = q.job_completed_entry_by) AS job_completed_entry_by,status from sha_request q order by id desc");
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
