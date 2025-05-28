using DETP.auth;
using DETP.Constant;
using DETP.data;
using DETP.model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DETP.Controllers
{
    public class QAObservation : Controller
    {

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public QAObservation(ILogger<QAObservation> logger, IConfiguration configuration, ApplicationDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {

            if (
                HttpContext.Session.GetString("app_name").Equals("QA") && HttpContext.Session.GetString("role_name").Equals("Admin")
                || HttpContext.Session.GetString("role_name").Equals("Super Admin")
                || HttpContext.Session.GetString("app_name").Equals("QA") && HttpContext.Session.GetString("role_name").Equals("QA Officer")
                || HttpContext.Session.GetString("app_name").Equals("QA") && HttpContext.Session.GetString("role_name").Equals("EIC-DETP")
                || _configuration["QaLogUser"].Split(",").Contains(HttpContext.Session.GetString("pno")
                ))
            {

                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "Log QA Observation";
                    ViewBag.departments = _context.Department.ToList();


                    ViewBag.appUsers = _context.Users
                        .Join(
                            _context.UserRoles,
                            userRoles => userRoles.UserId,
                            users => users.UserId,
                            (user, userRole) => new User()
                            {
                                RoleId = userRole.RoleId,
                                UserId = user.UserId,
                                App = user.App,
                                Department = user.Department,
                                Email = user.Email,
                                ForceReset = user.ForceReset,
                                Name = user.Name,
                                NotedMail = user.NotedMail,
                                PNo = user.PNo,
                            }
                        )
                        .Where(x => !new List<int> { RoleConst.BUSINESS_HEAD, RoleConst.EIC_DETP, RoleConst.HEAD_DETP, RoleConst.QA_OFFICER }.Contains(x.RoleId.Value))
                        .Where(x => x.App == "QA")
                        .ToList();


                    ViewBag.headDetp = GetHeadDetp();

                    sqlhelp.fetch1($"Select users.user_id,users.name,users.email,users.pno from users INNER JOIN user_role ON user_role.user_id = users.user_id where users.app='QA' AND user_role.role_id={RoleConst.BUSINESS_HEAD}");
                    ViewBag.businessHeads = _context.Users
                        .Join(
                            _context.UserRoles,
                            userRoles => userRoles.UserId,
                            users => users.UserId,
                            (user, userRole) => new User()
                            {
                                RoleId = userRole.RoleId,
                                UserId = user.UserId,
                                App = user.App,
                                Department = user.Department,
                                Email = user.Email,
                                ForceReset = user.ForceReset,
                                Name = user.Name,
                                NotedMail = user.NotedMail,
                                PNo = user.PNo,
                            }
                        )
                        .Where(x => x.RoleId == RoleConst.BUSINESS_HEAD)
                        .Where(x => x.App == "QA")
                        .ToList();

                    ViewBag.loggedUser = _context.Users.Where(x => x.UserId == HttpContext.Session.GetInt32("user")).FirstOrDefault();


                    var divisionList = _context.Divisions.ToList();
                    ViewBag.divisions = divisionList;

                    return View();
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            else
                return RedirectToAction("Login", "Account");
        }

        public ActionResult SubmitQA(
            long division_id,
            int department,
            string site,
            string location,
            string nature_of_work,
            string type_of_observation,
            string log_non_confirmance,
            string log_confirmance,
            string compliance_target_date,
            string type_of_confirmance,
            string nature_of_confirmance,
            string standard,
            string basics,
            string job,
            string vendor_code,
            string vendor_name,
            string p_o_no,
            string site_incharge,
            string head_detp,
            string project_incharge,
            string department_head,
            string business_head,
            string qa_officer,
            List<string> attachments,
            string observation_by,
            string observation_date,
            int number_of_observation,
            string area_of_concern,
            int count_of_observation
            )
        {
            
            return SubmitApi(division_id, department, site, location, nature_of_work, type_of_observation, log_non_confirmance, log_confirmance, compliance_target_date, type_of_confirmance, nature_of_confirmance, standard, basics, job, vendor_code, vendor_name, p_o_no, site_incharge, head_detp, project_incharge, department_head, business_head, qa_officer, attachments, observation_by, observation_date, number_of_observation, area_of_concern, count_of_observation);
        }
        [HttpPost]

        public JsonResult SubmitApi(
            long division_id,
            int department,
            string site,
            string location,
            string nature_of_work,
            string type_of_observation,
            string log_non_confirmance,
            string log_confirmance,
            string compliance_target_date,
            string type_of_confirmance,
            string nature_of_confirmance,
            string standard,
            string basics,
            string job,
            string vendor_code,
            string vendor_name,
            string p_o_no,
            string site_incharge,
            string head_detp,
            string project_incharge,
            string detp_hod,
            string business_head,
            string qa_officer,
            List<string> attachments,
            string observation_by,
            string observation_date,
            int number_of_observation,
            string area_of_concern,
            int count_of_observation)
        {
            if (site_incharge == "Select")
            {
                return Json(new { status = false, message = "Select Site Incharge" });
            }
            if (project_incharge == "Select")
            {
                return Json(new { status = false, message = "Select Project Incharge" });
            }
            string visit_no = "";
            string status = "";
            try
            {
                int mm = DateTime.Now.Month;
                int yy = DateTime.Now.Year;

                var qaObseration = _context.QAObservations
                    .Where(x => x.DepartmentId == department.ToString())
                    .Where(x => x.LoggedDate.Value.Month == mm && x.LoggedDate.Value.Year == yy)
                    .OrderByDescending(x => x.SerialNo).FirstOrDefault();

                long serial_no = 0;
                if (qaObseration != null)
                {
                    string visitNo = qaObseration.VisitNo;
                    string lastPart = visitNo.Split("/").Last();
                    serial_no = long.Parse(lastPart);
                }

                serial_no += 1;

                var dept = _context.Department.FirstOrDefault(x => x.Id == department);
                visit_no = dept.Abbr + "/" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yy") + "/" + serial_no;

                if (type_of_observation == "Non Confirmnace")
                {
                    if (nature_of_confirmance == "Critical")
                    {
                        if (job == "Yes")
                        {
                            status = "With Dept HOD";
                        }
                        else
                        {
                            status = "With Head DETP";
                        }
                    }

                    else
                        status = "With Site Incharge";
                }
                else if (type_of_observation == "Good Observation")
                {
                    status = "Closed";
                }
                String ldt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var observation = new model.QAObservation
                {
                    VisitNo = visit_no,
                    LoggedDate = DateTime.Now,
                    DepartmentId = department.ToString(),
                    Status = status,
                    Site = site,
                    Location = location,
                    NatureOfWork = nature_of_work,
                    TypeOfObservation = type_of_observation,
                    LogNonConfirmance = log_non_confirmance,
                    LogConfirmance = log_confirmance,
                    ComplianceTargetDate = DateTime.Parse(compliance_target_date ?? DateTime.MinValue.ToString()),
                    TypeOfConfirmance = type_of_confirmance,
                    NatureOfConfirmance = nature_of_confirmance,
                    Standard = standard,
                    Basics = basics,
                    Job = job,
                    VendorCode = vendor_code,
                    VendorName = vendor_name,
                    Pno = p_o_no,
                    SiteInchargeId = int.Parse(site_incharge),
                    HeadDetpId = int.Parse(head_detp),
                    HodQaShaId = GetHodQaSha().UserId,
                    ProjectInchargeId = int.Parse(project_incharge),
                    DeptHodId = int.Parse(detp_hod),
                    BusinessHeadId = int.Parse(business_head),
                    QaOfficerId = int.Parse(qa_officer),
                    ObservationById = int.Parse(observation_by),
                    ObservationDate = DateTime.Parse(observation_date),
                    DivisionId = division_id,
                    NumberOfObservation = number_of_observation,
                    AreaOfConcern = area_of_concern
                };
                _context.QAObservations.Add(observation);
                _context.SaveChanges();



                int serial_no1 = observation.SerialNo;
                int? mailto = null;
                String mailfrom = HttpContext.Session.GetInt32("user").ToString();
                if (observation.TypeOfObservation.Equals("Non Confirmnace"))
                {
                    if (observation.NatureOfConfirmance.Equals("Critical"))
                    {
                        if (observation.Job == "Yes")
                        {
                            _context.QAFlows.Add(new QAFlow
                            {
                                ObservationId = serial_no1,
                                TableName = "dept_hod",
                                FromId = HttpContext.Session.GetInt32("user"),
                                ToId = observation.DeptHodId,
                                Date = DateTime.Now,
                                Completed = "N",

                            });
                            mailto = observation.DeptHodId;
                            
                            string deptName = "";

                            try
                            {
                                deptName = dept.Name;
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("accessing index");
                            }
                            var sincharge = _context.Users.Where(x => x.UserId == observation.SiteInchargeId).FirstOrDefault();
                            
                            string user = "";

                            try
                            {
                                user = sincharge.UserId + " / " + sincharge.PNo;
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("accessing index");
                            }
                            var usersIds = new List<int?> { observation.SiteInchargeId, observation.ProjectInchargeId, observation.DeptHodId };
                            var users = _context.Users.Where(x => usersIds.Contains(x.UserId)).ToList();
                            

                            var url = $"{_configuration["SiteDetails:Production_URL"]}/EditObservation?serial_no={serial_no1}";

                            string body = GetMailBody(observation.TypeOfObservation, deptName, observation.Site, observation.Location, user, url);

                            sqlhelp.fetch1("SELECT email FROM job_stop_email");
                            var cc = "";
                            foreach (DataRow item in sqlhelp.datatable1.Rows)
                            {
                                cc += item.ItemArray[0].ToString() + ",";
                            }
                            string to = string.Join(",", users.Select(x => x.Email).ToList());
                            to += ",";

                            mail.SendMail(to, "Information for Job Stop in Poor Quality of Work", body, cc);
                        }
                        else
                        {
                            _context.QAFlows.Add(new QAFlow
                            {
                                ObservationId = serial_no1,
                                TableName = "head_detp",
                                FromId = HttpContext.Session.GetInt32("user"),
                                ToId = observation.HeadDetpId,
                                Date = DateTime.Now,
                                Completed = "N",

                            });
                            
                            mailto = observation.HeadDetpId;
                        }
                    }
                    else
                    {
                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = serial_no1,
                            TableName = "site_incharge",
                            FromId = HttpContext.Session.GetInt32("user"),
                            ToId = observation.SiteInchargeId,
                            Date = DateTime.Now,
                            Completed = "N",

                        });
                        mailto = observation.SiteInchargeId;
                    }

                    try
                    {
                        var usr = _context.Users.Where(x => x.UserId == mailto).FirstOrDefault();
                        if (usr != null)
                        {
                            String email_id = usr.Email;

                            String name = usr.Name;

                            int obsno = observation.SerialNo;
                            String body = "Dear <i>" + name + $"</i>,<br><br>You have an action pending on an observation.<br><br>Please click on following button:-<br><br> <b><a href='{_configuration["SiteDetails:Production_URL"]}/EditObservation?serial_no=" + obsno.ToString() + "&user_id=" + mailto + "' target='_blank'>VIEW OBSERVATION</a></b><br><br>Thanks,<br>QA- DETP<br>Tata Steel UISL";
                            mail.SendMail(email_id, "QA officer logged non-conformance in your area", body);
                        }

                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                    _context.SaveChanges();
                }
                _context.SaveChanges();

                foreach (string att1 in attachments)
                {
                    if (att1 == null)
                    {
                        continue;
                    }

                    _context.QaAtts.Add(new QaAtt
                    {
                        Data = att1,
                        Type = "observation",
                        TypeId = observation.SerialNo
                    });
                }


            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Json(new { status = false, message = "fail2" });
            }
            return Json(new { status = true, message = "success", visit_no });
        }

        private string GetMailBody(string observationType, string department, string siteName, string location, string siteIncharge, string url)
        {
            return $@"Dear Sir/Mam,
                    <br />PFB-Job Stop Information
                    <br />Type of Observation - {observationType}
                    <br/>Department - {department} 
                    <br />Site/Location - {siteName}/{location}
                    <br />Site incharge - {siteIncharge}
                    <br />Brief Description - Job stopped due to non-conformance in quality of work
                    <br />
                    <a href='{url}'> {url}</a>
                    ";
        }

        private User GetHodQaSha()
        {
            var role = _context.Roles.Where(x => x.Name == "HoD QA & SHA").FirstOrDefault();
            var userRole = _context.UserRoles.Where(x => x.RoleId == role.Id).FirstOrDefault();
            return _context.Users.Where(x => x.UserId == userRole.UserId).FirstOrDefault();
        }

        private User GetHeadDetp()
        {
            return _context.Users.Where(x => x.PNo == "159249").FirstOrDefault();
        }
    }
}
