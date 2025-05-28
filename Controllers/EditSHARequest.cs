using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Extensions.Logging;
using DETP.data;

namespace DETP.Controllers
{
    public class EditSHARequest : Controller
    {
        private readonly ILogger<EditSHARequest> _logger;
        private readonly ApplicationDbContext _context;
        public EditSHARequest(ILogger<EditSHARequest> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index(String serial_no)
        {
            if (HttpContext.Session.Get("user") != null)
            {
                if (!HttpContext.Session.GetString("role_name").Equals("Super Admin"))
                {
                    if (!HttpContext.Session.GetString("role_name").Equals("Admin"))
                    {
                        if (HttpContext.Session.GetString("role_name").Equals("Customer"))
                        {
                            sqlhelp.fetch1("select request_by from sha_request WHERE id='" + serial_no + "'");
                            String a = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                            String b = HttpContext.Session.GetString("logged_user");
                            if (!a.Equals(b))
                            {
                                return RedirectToAction("Login", "Account");
                            }
                        }
                    }
                }

                DataTable request = null,test = null,sample=null,photos=null,result=null;
                sqlhelp.fetch1("select *,(select department_abbr from department where department_id = q.department) as department,(select name from users where user_id = q.sha_team_decision_by) as decision_by_name,(select name from users where user_id = q.visit_by) as vi_visit_by_name,(select name from users where user_id = q.plan_test_date_entry_by) as ptdate_visit_by_name,(select name from users where user_id = q.job_completed_entry_by) as jc_by_name from sha_request q WHERE id='" + serial_no + "'");
                String obvid = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                if (sqlhelp.datatable1.Rows.Count > 0)
                {
                    request = sqlhelp.datatable1;
                    ViewBag.Title = sqlhelp.datatable1.Rows[0].ItemArray[3].ToString();
                   // if (!sqlhelp.datatable1.Rows[0].ItemArray[30].ToString().Equals("01-01-1900 00:00:00"))
                   // {
                        sqlhelp.fetch1("select * from sha_test_data WHERE sha_request_id='" + obvid + "'");
                    if (sqlhelp.datatable1.Rows.Count > 0)
                    {
                        test = sqlhelp.datatable1;
                    }
                        sqlhelp.fetch1("select * from sha_sample WHERE sha_request_id='" + obvid + "'");
                    if (sqlhelp.datatable1.Rows.Count > 0)
                    { 
                        sample = sqlhelp.datatable1;
                        }
                    sqlhelp.fetch1("select * from sha_test_result WHERE obv_id='" + obvid + "'");
                    if (sqlhelp.datatable1.Rows.Count > 0)
                    {
                        result = sqlhelp.datatable1;
                    }
                    //}
                    sqlhelp.fetch1("select * from sha_request q WHERE id='" + serial_no + "'");

                    if (!sqlhelp.datatable1.Rows[0].ItemArray[24].ToString().Equals("0"))
                    {
                        sqlhelp.fetch1("select * from sha_photos WHERE observation_id='" + sqlhelp.datatable1.Rows[0].ItemArray[0] + "'");
                        photos = sqlhelp.datatable1;
                    }

                                    }
                else
                {
                    return RedirectToAction("Login", "Account");

                }
                ViewBag.Attachment = _context.ShaRequestImages.Where(x => x.ShaRequestId == int.Parse(serial_no)).ToList();
                Tuple<DataTable, DataTable, DataTable, DataTable, DataTable> values = new(request, test, sample,photos,result);
                return View(values);
            }
            else
                return RedirectToAction("Login", "Account");
        }
        public ActionResult Update(String decision, String remarks, String planned_visit_date, List<string> observation, List<string> photos, String vircompleted, String acompleted,String ptdate,List<string>tname, List<string> tno, List<string> sname, List<string> sno, List<string> tr, List<int> lineno,List<int> old_lineno, List<string> old_t_id, List<string> old_tr, String jobc, String status, String request_id, String ac,String arrdes)
        {
                String status1 = "";
            String mailsend = "yes";
            if (status.Equals("Pending from SHA Team"))
            {
                if (decision.Equals("Accepted"))
                {
                    status1 = "Pending for Visual Inspection";
                }
                else
                {
                    status1 = "Rejected";
                     }
                sqlhelp.performAction("UPDATE sha_request SET sha_team_decision='" + decision + "',sha_team_decision_by='" + HttpContext.Session.GetInt32("user") + "',sha_team_decision_datetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',remarks='" + remarks + "',planned_visit_date='" + planned_visit_date + "',status='" + status1 + "' WHERE id='" + request_id + "'");
                 

            } else if (status.Equals("Pending for Visual Inspection"))
            {
                String rdt = "1900-01-01 00:00:00";
                if (ac.Equals("fwd"))
                {
                    status1 = "Approval Pending for Visual Inspection";
                    rdt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                }
                else {
                    status1 = "Pending for Visual Inspection";
                    mailsend = "no";
                }
               sqlhelp.performAction("UPDATE sha_request SET visit_observation_details='" + observation + "',visit_by='" + HttpContext.Session.GetInt32("user") + "',actual_visit_datetime='" + rdt + "',status='" + status1 + "' WHERE id='" + request_id + "'");
                int k = 0;
               
                    foreach (String img1 in photos)
                    {
                    if (img1 != null)
                    {
                        sqlhelp.insert("sha_photos", img1.ToString(), request_id, observation[k]);
                        k++;
                    }
                    }
               

            }
            else if (status.Equals("Approval Pending for Visual Inspection"))
            {
                if (vircompleted.Equals("Yes"))
                {
                    status1 = "Pending for Requester Confirmation";
                 
                }
                else
                {
                    status1 = "Approval Pending for Visual Inspection";

                }
                sqlhelp.performAction("UPDATE sha_request SET rcompleted_datetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',status='" + status1 + "' WHERE id='" + request_id + "'");
            }
            else if (status.Equals("Pending for Requester Confirmation"))
            {
                if (acompleted.Equals("Yes"))
                {
                    status1 = "Pending for Test Plan Date Confirmation";
                }
                else
                {
                    status1 = "Pending for Requester Confirmation";
                }
                sqlhelp.performAction("UPDATE sha_request SET rconfirmation_datetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',arrdes='"+arrdes+"',status='" + status1 + "' WHERE id='" + request_id + "'");
            }
            else if (status.Equals("Pending for Test Plan Date Confirmation"))
            {
               status1 = "Pending for Test Data";
               
                sqlhelp.performAction("UPDATE sha_request SET plan_test_date='"+ptdate+ "',plan_test_date_entry_by='"+ HttpContext.Session.GetInt32("user") + "',plan_test_date_entry_datetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',status='" + status1 + "' WHERE id='" + request_id + "'");
            }
            else if (status.Equals("Pending for Test Data"))
            {
                String rdt = "1900-01-01 00:00:00";
                if (ac.Equals("fwd"))
                {
                    status1 = "Pending for Test Results";
                    rdt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                }
                else
                {
                    status1 = "Pending for Test Data";
                    mailsend = "no";

                }

                sqlhelp.performAction("UPDATE sha_request SET actual_test_datetime='" + rdt + "',status='" + status1 + "' WHERE id='" + request_id + "'");
                int i = 0;
                foreach (String tname1 in tname)
                {
                    
                   
                    if (tname1 != null)
                    {
                        String user_name = HttpContext.Session.GetInt32("user").ToString();
                        sqlhelp.insert("sha_test_data", tname1.ToString(), tno[i].ToString(), "", "", "", request_id, user_name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                        i++;
                    
                }
                int j = 0;

                foreach (String sname1 in sname)
                {


                    if (sname1 != null)
                    {
                    String user_name = HttpContext.Session.GetInt32("user").ToString();
                        sqlhelp.insert("sha_sample", sname1.ToString(), sno[j].ToString(), request_id, user_name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                        j++;

                }
            }
            else if (status.Equals("Pending for Test Results"))
            {
                if (ac.Equals("fwd"))
                {
                    status1 = "Pending for Final Report";
                }
                else
                {
                    status1 = "Pending for Test Results";
                    mailsend = "no";

                }
                sqlhelp.performAction("UPDATE sha_request SET status='" + status1 + "' WHERE id='" + request_id + "'");
                int i = 0;
                int j = 0;
                foreach (int old_lineno1 in old_lineno)
                {
                    if (!old_lineno1.Equals(""))
                    {
                        String user_name = HttpContext.Session.GetInt32("user").ToString();
                        //sqlhelp.performAction("UPDATE sha_test_data SET test_result='" + + "',test_result_by_userid='"+user_name+ "',test_result_entry_datetime='"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id='"+old_t_id1+"'");
                        if (old_tr[j] != null)
                        {
                            sqlhelp.insert("sha_test_result", old_t_id[old_lineno1], old_tr[j].ToString(), user_name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request_id);
                        }
                        j++;
                    }

                }
                foreach (String tname1 in tname)
                {
                    var dtt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    String user_name = HttpContext.Session.GetInt32("user").ToString();
                    if (tname1 !=null)
                    {
                        sqlhelp.insert("sha_test_data", tname1?.ToString(), tno[i]?.ToString(), "", user_name, dtt, request_id, user_name, dtt);
                    }
                        i++;

                }
                    int l = 0;
                foreach (int lineno1 in lineno)
                {
                    if (!lineno1.Equals(""))
                    {
                        var dtt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        String user_name = HttpContext.Session.GetInt32("user").ToString();

                        if (tr[l] != null)
                        {
                        sqlhelp.fetch1("select id from sha_test_data WHERE test_name='" + tname[lineno1]?.ToString() + "' AND test_no='" + tno[lineno1]?.ToString() + "' AND test_result_by_userid='" + user_name + "' AND test_result_entry_datetime='" + dtt + "' AND sha_request_id='" + request_id + "'");
                        var latesttestid = sqlhelp.datatable1.Rows[0].ItemArray[0];
                            sqlhelp.insert("sha_test_result", latesttestid.ToString(), tr[l].ToString(), user_name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request_id);
                        }
                            l++;
                    }
                }

            }
            else if (status.Equals("Pending for Final Report"))
            {
                if (jobc.Equals("Yes"))
                {
                    status1 = "Job Completed";
                }
                else
                {
                    status1 = "Pending for Final Report";
                }
                String user_name = HttpContext.Session.GetInt32("user").ToString();

                sqlhelp.performAction("UPDATE sha_request SET job_completed_datetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',job_completed_entry_by='"+user_name+"',status='" + status1 + "' WHERE id='" + request_id + "'");
            }
            if (mailsend.Equals("yes"))
            {
                sqlhelp.fetch1("select request_by,job_request_no,status,id,contact_person_email from sha_request where id = '" + request_id + "'");
                String mailto = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                String job_request_no = sqlhelp.datatable1.Rows[0].ItemArray[1].ToString();
                String jbstatus = sqlhelp.datatable1.Rows[0].ItemArray[2].ToString();
                String jbid = sqlhelp.datatable1.Rows[0].ItemArray[3].ToString();
                String contact_person_email = sqlhelp.datatable1.Rows[0].ItemArray[4].ToString();
                String mailcon = "";
                if (jbstatus.Equals("Pending for Requester Confirmation"))
                {
                    mailcon = "<br>Remarks - Please make neccassary arrangement at the site.";
                }
                sqlhelp.fetch1("select email,name from users where user_id = '" + mailto + "'");
                String email_id = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                String name = sqlhelp.datatable1.Rows[0].ItemArray[1].ToString();
                String link = "http://202.78.235.20/DETP/EditSHARequest?serial_no=" + jbid;
                String body = "Dear Sir,<br><br>Please find the details of the following:-<br>Request No :-" + job_request_no + "<br>Request Status :- " + jbstatus + "" + mailcon + "<br><br>Please click on following button:-<br><br> <b><a href='" + link + "' target='_blank'>VIEW REQUEST</a></b><br><br>Thanks,<br>SHA Application<br>TataSteel UISL";
                String subject = "Notification from SHA (Request No -" + job_request_no + ")";
                mail.SendMail(email_id, subject, body);
                if (!email_id.Equals(contact_person_email))
                {
                    mail.SendMail(contact_person_email, subject, body);
                }
            }
            return Json(new { status = true, message = "success" });

        }
        public ActionResult DeletePhoto(String id, String observation_id)
        {
            sqlhelp.performAction("DELETE FROM sha_photos WHERE id='" + id + "'");
            return RedirectToAction("Index", "EditSHARequest", new { serial_no = observation_id });
        }
        public ActionResult DeleteTest(String id, String observation_id)
        {
            sqlhelp.performAction("DELETE FROM sha_test_data WHERE id='" + id + "'");
            sqlhelp.performAction("DELETE FROM sha_test_result WHERE test_id='" + id + "'");
            return RedirectToAction("Index", "EditSHARequest", new { serial_no = observation_id });
        }
        public ActionResult DeleteSample(String id, String observation_id)
        {
            sqlhelp.performAction("DELETE FROM sha_sample WHERE id='" + id + "'");
            return RedirectToAction("Index", "EditSHARequest", new { serial_no = observation_id });
        }
        public ActionResult DeleteResult(String id, String observation_id)
        {
            sqlhelp.performAction("DELETE FROM sha_test_result WHERE id='" + id + "'");
            return RedirectToAction("Index", "EditSHARequest", new { serial_no = observation_id });
        }
    }
}
