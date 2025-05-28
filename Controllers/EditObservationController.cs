using DETP.auth;
using DETP.Constant;
using DETP.data;
using DETP.helpers;
using DETP.model;
using DETP.requests.editObservation;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DETP.Controllers
{
    [Authorize(app: new string[] { "QA" })]
    public class EditObservationController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<EditObservationController> _logger;
        private readonly ApplicationDbContext _context;
        sqlhelp _ = new();

        public EditObservationController(
            IConfiguration configuration, 
            ILogger<EditObservationController> logger, 
            ApplicationDbContext context
            )
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index(String serial_no)
        {

            int user_id = 0;

            user_id = (int)HttpContext.Session.GetInt32("user");

            ViewBag.Title = "Observation";
            List<BaseModel> values = new();
            int from_id = 1, current_flow_id = 0, to_id = 0;

            var QAFlow = GetQAFlow(serial_no);

            List<QaAtt> QAAtt = GetQAAtt(int.Parse(serial_no));

            var observation = _context.QAObservations
                .Include(x => x.Division)
                .Include(x => x.SiteIncharge)
                .Include(x => x.ProjectIncharge)
                .Include(x => x.DeptHod)
                .Include(x => x.HeadDetp)
                .Include(x => x.BusinessHead)
                .Include(x => x.ObservatioinBy)
                .Include(x => x.HodQaSha)
                .FirstOrDefault(x => x.SerialNo == int.Parse(serial_no));

            if (observation.HodQaShaId == null)
            {
                var user1 = _context.Users.Where(x => x.PNo == "155670").FirstOrDefault();
                if (user1 == null)
                {
                    return Json(new { message = "Something went wrong!" });
                }
                observation.HodQaShaId = user1.UserId;

            }
            string status = observation.Status;
            observation.Dept = _context.Department.Where(x => x.Id.ToString() == observation.DepartmentId).FirstOrDefault();
            observation.Atts = QAAtt;
            ViewBag.observation = observation;



            for (int i = 0; i < QAFlow.Count; i++)
            {
                int flow_id = 0;
                var k = 0;
                if (i != 0)
                {
                    k = i - 1;
                }
                if (QAFlow[i].Completed.Equals("Y"))
                {
                    try
                    {
                        // for head_detp
                        if (QAFlow[i].TableName.Equals("head_detp"))
                        {
                            flow_id = QAFlow[i].Id;
                            var data = _context.HeadDetps
                                .Include(x => x.DecisionBy)
                                .Where(x => x.FlowId == flow_id)
                                .FirstOrDefault();
                            if (data != null)
                                values.Add(data);
                        }
                    }
                    catch (Exception e) { _logger.LogError(e.StackTrace); }

                    try
                    {

                        if (QAFlow[i].TableName.Equals("hod_qa_sha"))
                        {
                            flow_id = QAFlow[i].Id;
                            var data = _context.HodQaSha
                                .Include(x => x.DecisionBy)
                                .Where(x => x.FlowId == flow_id)
                                .FirstOrDefault();
                            if (data != null)
                                values.Add(data);
                        }
                    }
                    catch (Exception e) { _logger.LogError(e.StackTrace); }


                    // for business_head
                    try
                    {
                        if (QAFlow[i].TableName.Equals("business_head"))
                        {
                            flow_id = QAFlow[i].Id;

                            var data = _context.Set<BusinessHead>()
                                .Include(x => x.DecisionBy)
                                .Include(x => x.AssignTo)
                                .Where(x => x.FlowId == flow_id)
                                .FirstOrDefault();
                            if (data != null)
                            {
                                data.Atts = _context.QaAtts
                                        .Where(x => x.TypeId == flow_id && x.Type == "flow")
                                        .ToList();
                                values.Add(data);
                            }
                        }
                    }
                    catch (Exception e) { _logger.LogError(e.StackTrace); }

                    try
                    {

                        if (QAFlow[i].TableName.Equals("assignee"))
                        {
                            flow_id = QAFlow[i].Id;

                            var data = _context.Assignees
                                .Include(x => x.DecisionBy)
                                .Where(x => x.FlowId == flow_id)
                                .FirstOrDefault();
                            if (data != null)
                            {
                                data.Atts = _context.QaAtts
                                        .Where(x => x.TypeId == flow_id && x.Type == "flow")
                                        .ToList();
                                values.Add(data);
                            }
                        }
                    }
                    catch (Exception e) { _logger.LogError(e.StackTrace); }

                    try
                    {
                        if (QAFlow[i].TableName.Equals("site_incharge"))
                        {
                            flow_id = QAFlow[i].Id;
                            var data = _context.SiteIncharges
                                .Include(x => x.DecisionBy)
                                .Where(x => x.FlowId == flow_id)
                                .FirstOrDefault();
                            if (data != null)
                            {
                                data.Atts = _context.QaAtts
                                        .Where(x => x.TypeId == flow_id && x.Type == "flow")
                                        .ToList();
                                values.Add(data);
                            }
                        }
                    }
                    catch (Exception e) { _logger.LogError(e.StackTrace); }


                    try
                    {
                        if (QAFlow[i].TableName.Equals("project_incharge"))
                        {
                            flow_id = QAFlow[i].Id;

                            var data = _context.ProjectIncharges
                                .Include(x => x.DecisionBy)
                                .Where(x => x.FlowId == flow_id)
                                .FirstOrDefault();
                            if (data != null)
                            {
                                data.Atts = _context.QaAtts
                                        .Where(x => x.TypeId == flow_id && x.Type == "flow")
                                        .ToList();
                                values.Add(data);
                            }

                        }
                    }
                    catch (Exception e) { _logger.LogError(e.StackTrace); }


                    try
                    {

                        if (QAFlow[i].TableName.Equals("dept_hod"))
                        {
                            flow_id = QAFlow[i].Id;

                            var data = _context.DeptHods
                                .Include(x => x.DecisionBy)
                                .Where(x => x.FlowId == flow_id)
                                .FirstOrDefault();
                            if (data != null)
                            {
                                data.Atts = _context.QaAtts
                                        .Where(x => x.TypeId == flow_id && x.Type == "flow")
                                        .ToList();
                                values.Add(data);

                            }

                        }
                    }

                    catch (Exception e)
                    {
                        _logger.LogError(e.StackTrace);
                    }




                    try
                    {
                        if (QAFlow[i].TableName.Equals("qa_officer"))
                        {
                            flow_id = QAFlow[i].Id;

                            var data = _context.QAOfficers
                                .Include(x => x.DecisionBy)
                                .Where(x => x.FlowId == flow_id)
                                .FirstOrDefault();
                            if (data != null)
                            {
                                data.Atts = _context.QaAtts
                                        .Where(x => x.TypeId == flow_id && x.Type == "flow")
                                        .ToList();
                                values.Add(data);
                            }

                        }
                    }
                    catch (Exception e) { _logger.LogError(e.StackTrace); }

                    try
                    {
                        if (QAFlow[i].TableName.Equals("eic_detp") && QAFlow[i].Completed.Equals("Y"))
                        {
                            flow_id = QAFlow[i].Id;

                            var data = _context.QAOfficers
                                .Include(x => x.DecisionBy)
                                .Where(x => x.FlowId == flow_id)
                                .FirstOrDefault();
                            if (data != null)
                            {
                                values.Add(data);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.StackTrace);
                    }
                }
                //for current flow
                try
                {
                    if (QAFlow[i].Completed.Equals("N"))
                    {
                        current_flow_id = QAFlow[i].Id;
                        to_id = QAFlow[i].To.UserId;
                    }
                    else
                    {
                        if (QAFlow[i].TableName == "dept_hod")
                        {
                            from_id = RoleConst.DEPARTMENT_HOD;
                        }
                        else if (QAFlow[i].TableName == "assignee")
                        {
                            from_id = RoleConst.ASSIGNEE_SECTION;
                        }
                        else if (QAFlow[i].TableName == "head_detp")
                        {
                            from_id = RoleConst.HEAD_DETP;
                        }
                        else if (QAFlow[i].TableName == "eic_detp")
                        {
                            from_id = RoleConst.EIC_DETP;
                        }
                        else if (QAFlow[i].TableName == "business_head")
                        {
                            from_id = RoleConst.BUSINESS_HEAD;
                        }
                        else if (QAFlow[i].TableName == "project_incharge")
                        {
                            from_id = RoleConst.PROJECT_INCHARGE;
                        }
                        else if (QAFlow[i].TableName == "site_incharge")
                        {
                            from_id = RoleConst.SITE_INCHARGE;
                        }
                        else if (QAFlow[i].TableName == "qa_officer")
                        {
                            from_id = RoleConst.QA_OFFICER;
                        }
                        else if (QAFlow[i].TableName == "hod_qa_sha")
                        {
                            from_id = RoleConst.HOD_QA_SHA;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.StackTrace);
                }
            }



            var role = GetRoleByObservation(observation.SerialNo);

            var currentUserHas = to_id == user_id;
            var currentState = FindState(observation.NatureOfConfirmance, observation.Job, from_id, user_id, observation.SerialNo);



            var user = _context.Users.Where(x => x.UserId == user_id).FirstOrDefault();
            string user_name = user.Name;

            var dt = new CurrentFlow
            {
                Name = user_name,
                UserId = user_id,
                FromId = from_id,
                ObservationId = serial_no,
                FlowId = current_flow_id,
                Critical = observation.IsCritical,
                CurrentState = currentState
            };

            if (status.Equals("With Head DETP") && currentUserHas)
            {
                dt.Department = "Head DETP";
                dt.SubmitTo = observation.BusinessHeadId;
            }

            if (status.Equals("With Business Head") && currentUserHas)
            {

                dt.Department = "Business Head";
                dt.SubmitTo = observation.HodQaShaId;

                if (from_id.Equals("4") || from_id.Equals("13") || from_id.Equals("7") || from_id.Equals("1") || from_id.Equals("13"))
                {
                    ViewBag.AssigneeNames = _context.Users.Where(x => x.App == "QA");
                }
            }


            if (status.Equals("With Assignee Section") && currentUserHas)
            {

                string submit_to = observation.BusinessHeadId.ToString();

                dt.Department = "Assignee Section";

                dt.SubmitTo = observation.BusinessHeadId;

            }

            if (status.Equals("With Site Incharge") && currentUserHas)
            {

                string submit_to = observation.QaOfficerId.ToString();
                dt.Department = "Site Incharge";

                dt.SubmitTo = observation.QaOfficerId;
            }

            if (status.Equals("With Project Incharge") && currentUserHas)
            {
                string submit_to = observation.QaOfficerId.ToString();

                dt.Department = "Project Incharge";

                dt.SubmitTo = observation.QaOfficerId;

            }

            if (status.Equals("With Dept HOD") && currentUserHas)
            {
                String submit_to = observation.QaOfficerId.ToString();

                dt.Department = "Dept HOD";
                dt.SubmitTo = observation.QaOfficerId;
                dt.JobStopped = GetJobStoped(serial_no);

            }

            if (status.Equals("With QA Officer") && !from_id.Equals("4") && currentUserHas)
            {
                String submit_to = observation.DeptHodId.ToString();

                dt.Department = "QA Officer";

                dt.SubmitTo = observation.DeptHodId;
            }

            if (status.Equals("With QA Officer") && from_id.Equals("4") && currentUserHas)
            {
                String submit_to = observation.DeptHodId.ToString();
                dt.Department = "H-QA Officer";
                dt.SubmitTo = observation.DeptHodId;
            }
            if (status.Equals("With EIC DETP") && currentUserHas)
            {
                var userRole = _context.UserRoles.Where(x => x.RoleId == 12).FirstOrDefault();
                dt.Department = "EIC DETP";
                dt.SubmitTo = userRole.UserId;

                dt.JobStopped = GetJobStoped(serial_no);
            }


            if (status.Equals("With HoD QA & SHA") && currentUserHas)
            {
                dt.Department = "HoD QA & SHA";
                var userRole = _context.UserRoles.Where(x => x.RoleId == 13).FirstOrDefault();
                dt.SubmitTo = userRole.UserId;

                dt.JobStopped = GetJobStoped(serial_no);
            }

            ViewBag.CurrentFlow = dt;
            ViewBag.AssigneeNames = _context.Users.Where(x => x.App == "QA").ToList();
            return View(values);


        }

        private string FindState(string natureOfConfirmance, string jobStopped, int from, int to, int observationId)
        {

            var roleFrom = from;
            var roleTo = GetRoleByObservation(observationId);

            if (
                natureOfConfirmance.ToLower().Equals("critical") && jobStopped.ToLower().Equals("yes"))
            {
                if (roleTo == RoleConst.DEPARTMENT_HOD)
                {
                    if (roleFrom == RoleConst.QA_OFFICER || roleFrom == RoleConst.HOD_QA_SHA || roleFrom == RoleConst.HEAD_DETP)
                    {
                        return State.CRITICAL_JOB_STOPPED_DEPT_HOD_FROM_START;
                    }
                    if (roleFrom == RoleConst.ASSIGNEE_SECTION)
                    {
                        return State.CRITICAL_JOB_STOPPED_DEPT_HOD_FROM_ASSIGNEE;
                    }
                }

                if (roleTo == RoleConst.ASSIGNEE_SECTION && roleFrom == RoleConst.DEPARTMENT_HOD)
                {
                    return State.CRITICAL_JOB_STOPPED_ASSIGNEE_FROM_HOD_DEPT;
                }

                if (roleTo == RoleConst.ASSIGNEE_SECTION && roleFrom == RoleConst.DEPARTMENT_HOD)
                {
                    return State.CRITICAL_JOB_STOPPED_ASSIGNEE_FROM_HOD_DEPT;
                }
                if (roleTo == RoleConst.ASSIGNEE_SECTION && roleFrom == RoleConst.DEPARTMENT_HOD)
                {
                    return State.CRITICAL_JOB_STOPPED_ASSIGNEE_FROM_HOD_DEPT;
                }
                if (roleTo == RoleConst.HEAD_DETP && roleFrom == RoleConst.DEPARTMENT_HOD)
                {
                    return State.CRITICAL_JOB_STOPPED_HEAD_DETP_FROM_DEPT_HOD;
                }
                if (roleTo == RoleConst.HOD_QA_SHA && roleFrom == RoleConst.HEAD_DETP)
                {
                    return State.CRITICAL_JOB_STOPPED_HOD_QA_SHA_FROM_HEAD_DETP;
                }
            }

            return null;
        }

        public ActionResult Submit(String data, String user_id, String to, String from, List<string> att, int observation_id, String flow_id, String within_slg)
        {
            String mailfrom = "";
            int? mailto = 0;
            string subject = "";
            var status = "";

            model.QAObservation observation = _context.QAObservations.Where(x => x.SerialNo == observation_id).FirstOrDefault();
            var loggedUserId = (int)HttpContext.Session.GetInt32("user");
            var currentFlow = _context.QAFlows.FirstOrDefault(x => x.Id.ToString() == flow_id);

            try
            {
                if (GetRoleByObservation(observation_id) == RoleConst.HOD_QA_SHA)
                {

                    if (!IsJobStopped(observation_id))
                    {
                        JToken token = JObject.Parse(data);
                        if (token.SelectToken("decision").ToString().Equals("Close"))
                        {
                            sqlhelp.insert("hod_qa_sha", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), "", "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                            sqlhelp.performAction("update qa_observation set status = 'Closed by HoD QA & SHA' where serial_no = '" + observation_id + "'");
                            status = AppConstants.ObservationStatus.CLOSED_BY_HOD_QA_SHA;
                            subject = "Non-conformance closed by With HoD QA & SHA";
                        }
                        else if (token.SelectToken("decision").ToString().Equals("Return QA"))
                        {
                            sqlhelp.insert("hod_qa_sha", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), "", "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                            sqlhelp.fetch1("select head_detp from qa_observation where serial_no = '" + observation_id + "' ");

                            mailto = observation.HeadDetpId;

                            sqlhelp.insert("qa_flow", observation_id.ToString(), "head_detp", user_id, to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                            sqlhelp.performAction("update qa_observation set status = 'With HoD QA & SHA' where serial_no = '" + observation_id + "'");
                            status = AppConstants.ObservationStatus.WITH_HOD_QA_SHA;
                            mailfrom = user_id;
                            subject = "Non-conformance returned by HoD QA & SHA.";
                        }
                        else
                        {
                            sqlhelp.insert("hod_qa_sha", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), "", "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                            sqlhelp.fetch1("select business_head from qa_observation where serial_no = '" + observation_id + "' ");

                            mailto = observation.BusinessHeadId;

                            sqlhelp.insert("qa_flow", observation_id.ToString(), "business_head", user_id, to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                            sqlhelp.performAction("update qa_observation set status = 'With Business Head' where serial_no = '" + observation_id + "'");
                            status = AppConstants.ObservationStatus.WITH_BUSINESS_HEAD;
                            mailfrom = user_id;
                            subject = "Non-conformance returned by HoD QA & SHA.";
                        }
                        sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                    }
                    else
                    {
                        JToken token = JObject.Parse(data);
                        if (token.SelectToken("decision").ToString().Equals("Close"))
                        {
                            sqlhelp.insert("hod_qa_sha", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), "", "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                            sqlhelp.performAction("update qa_observation set status = 'Closed by HoD QA & SHA' where serial_no = '" + observation_id + "'");
                            status = AppConstants.ObservationStatus.CLOSED_BY_HOD_QA_SHA;
                            subject = "Non -conformance closed by HoD QA & SHA";

                        }
                        else
                        {
                            sqlhelp.insert("hod_qa_sha", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), "", "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                            sqlhelp.fetch1("select dept_hod from qa_observation where serial_no = '" + observation_id + "' ");

                            mailto = observation.DeptHodId;

                            sqlhelp.insert("qa_flow", observation_id.ToString(), "dept_hod", user_id, to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                            sqlhelp.performAction("update qa_observation set status = 'With Dept HOD' where serial_no = '" + observation_id + "'");
                            status = AppConstants.ObservationStatus.WITH_DEPT_HOD;
                            mailfrom = user_id;
                            subject = "Non -conformance returned by HoD QA & SHA.";
                        }
                        sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                    }
                }

                //--------------------------------EIC submit end ---------------------


                else if (GetRoleByObservation(observation_id) == RoleConst.HEAD_DETP)
                {
                    JToken token = JObject.Parse(data);
                    if (!IsJobStopped(observation_id))
                    {
                        if (!from.Equals("5") && !token.SelectToken("qaaa").ToString().Equals("yes") && !from.Equals("13"))
                        {

                            if (token.SelectToken("decision").ToString().Equals("Rejected"))
                            {
                                sqlhelp.insert("head_detp", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), token.SelectToken("job")?.ToString(), "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());

                                sqlhelp.performAction("update qa_observation set status = 'Closed by Head DETP' where serial_no = '" + observation_id + "'");
                                status = "Closed by Head DETP";
                                subject = "Non -conformance closed by head DETP.";
                            }
                            else if (token.SelectToken("decision").ToString().Equals("Convert"))
                            {
                                sqlhelp.insert("head_detp", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), token.SelectToken("obv")?.ToString(), "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());

                                mailto = observation.SiteInchargeId;
                                sqlhelp.insert("qa_flow", observation_id.ToString(), "site_incharge", user_id, mailto.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                                sqlhelp.performAction("update qa_observation set nature_of_confirmance='" + token.SelectToken("obv")?.ToString() + "',status = 'With Site Incharge' where serial_no = '" + observation_id + "'");
                                status = "With Site Incharge";
                                mailfrom = user_id;
                                subject = "Input given by QA Sectional Head on non-conformance";
                            }
                            else
                            {
                                sqlhelp.insert("head_detp", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), token.SelectToken("job")?.ToString(), "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());

                                sqlhelp.insert("qa_flow", observation_id.ToString(), "business_head", user_id, to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                                sqlhelp.performAction("update qa_observation set status = 'With Business Head' where serial_no = '" + observation_id + "'");
                                status = "With Business Head";
                                mailfrom = user_id;
                                mailto = int.Parse(to);
                                subject = "Input given by QA Sectional Head on non-conformance";
                            }

                            sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                        }
                        else
                        {


                            if (token.SelectToken("decision").ToString().Equals("Ok and Closed"))
                            {
                                sqlhelp.insert("head_detp", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), "", "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                                sqlhelp.performAction("update qa_observation set status = 'Closed by Head DETP' where serial_no = '" + observation_id + "'");
                                status = "Closed by Head DETP";
                                subject = "Non -conformance closed by head DETP.";
                            }

                            if (token.SelectToken("decision").ToString().Equals("Not Satisfied") || token.SelectToken("decision").ToString().Equals("Need Clarification"))
                            {
                                sqlhelp.insert("head_detp", observation_id.ToString(), flow_id, "", "", token.SelectToken("decision").ToString(), token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                                sqlhelp.insert("qa_flow", observation_id.ToString(), "business_head", user_id, to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                                sqlhelp.performAction("update qa_observation set status = 'With Business Head' where serial_no = '" + observation_id + "'");
                                status = "With Business Head";
                                mailfrom = user_id;
                                mailto = int.Parse(to);
                                subject = "Non-conformance returned by QA Sectional Head.";
                            }

                            if (token.SelectToken("decision").ToString().Equals("Forward to QA"))
                            {
                                sqlhelp.insert("head_detp", observation_id.ToString(), flow_id, "", "", token.SelectToken("decision").ToString(), token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                                sqlhelp.fetch1("Select user_id from user_role where role_id = 13");
                                String submit_to = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                                sqlhelp.insert("qa_flow", observation_id.ToString(), "hod_qa_sha", user_id, submit_to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                                sqlhelp.performAction("update qa_observation set status = 'With HoD QA & SHA' where serial_no = '" + observation_id + "'");
                                status = "With HoD QA & SHA";

                                sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");

                                mailfrom = user_id;
                                mailto = int.Parse(submit_to);
                                subject = "Non-conformance returned by HoD QA & SHA.";
                            }

                            sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                        }
                    }
                    else
                    {
                        if (token.SelectToken("decision").ToString().Equals("Need Clarification"))
                        {
                            sqlhelp.insert("head_detp", observation_id.ToString(), flow_id, "", "", token.SelectToken("decision").ToString(), token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                            sqlhelp.fetch1($"Select TOP 1 dept_hod from qa_observation where serial_no={observation_id}");
                            String submit_to = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                            sqlhelp.insert("qa_flow", observation_id.ToString(), "dept_hod", user_id, submit_to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                            sqlhelp.performAction("update qa_observation set status = 'With Dept HOD' where serial_no = '" + observation_id + "'");
                            status = "With Dept HOD";
                            sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");

                            mailfrom = user_id;
                            mailto = int.Parse(submit_to);

                        }
                        else
                        {
                            sqlhelp.insert("head_detp", observation_id.ToString(), flow_id, "", "", token.SelectToken("decision").ToString(), token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                            sqlhelp.fetch1("Select user_id from user_role where role_id = 13");
                            String submit_to = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                            sqlhelp.insert("qa_flow", observation_id.ToString(), "hod_qa_sha", user_id, submit_to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                            sqlhelp.performAction("update qa_observation set status = 'With HoD QA & SHA' where serial_no = '" + observation_id + "'");
                            status = "HoD QA & SHA";
                            sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                            mailfrom = user_id;
                            mailto = int.Parse(submit_to);
                        }

                        sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                    }

                }

                else if (GetRoleByObservation(observation_id) == RoleConst.BUSINESS_HEAD)
                {
                    JToken token = JObject.Parse(data);

                    if (!observation.IsCritical)
                    {
                        if (token.SelectToken("assigned_to") != null)
                        {
                            int assigned_to = int.Parse(token.SelectToken("assigned_to").ToString());
                            _context.BusinessHeads.Add(new BusinessHead
                            {
                                ObservationId = observation_id,
                                FlowId = currentFlow.Id,
                                AssignToId = assigned_to,
                                TargetDate = token.SelectToken("target_date").ToString(),
                                Remarks = token.SelectToken("remarks").ToString(),
                                DecisionById = loggedUserId,
                                DecisionDate = DateTime.Now
                            });

                            _context.QAFlows.Add(new QAFlow
                            {
                                ObservationId = observation_id,
                                TableName = "assignee",
                                FromId = loggedUserId,
                                ToId = assigned_to,
                                Completed = "N",
                                Date = DateTime.Now
                            });
                            observation.Status = "With Assignee Section";
                            status = AppConstants.ObservationStatus.WITH_ASSIGNEE_SECTION;
                            currentFlow.Completed = "Y";
                            mailfrom = user_id;
                            mailto = assigned_to;
                            subject = "Input given by Business Head on non-conformance.";
                        }
                        else
                        {
                            _context.BusinessHeads.Add(new BusinessHead
                            {
                                ObservationId = observation_id,
                                FlowId = currentFlow.Id,
                                Remarks = token.SelectToken("remarks").ToString(),
                                DecisionById = loggedUserId,
                                DecisionDate = DateTime.Now,
                                Decision = token.SelectToken("decision").ToString()
                            });
                            if (token.SelectToken("decision").ToString().Equals("Return to assignee"))
                            {
                                int assignee_id = int.Parse(checkAssignee(Int16.Parse(flow_id)));
                                _context.QAFlows.Add(new QAFlow
                                {
                                    ObservationId = observation_id,
                                    TableName = "assignee",
                                    FromId = loggedUserId,
                                    ToId = assignee_id,
                                    Completed = "N",
                                    Date = DateTime.Now
                                });
                                observation.Status = "With Assignee Section";
                                status = AppConstants.ObservationStatus.WITH_ASSIGNEE_SECTION;
                                mailfrom = user_id;
                                mailto = assignee_id;
                                subject = "Input given by Business Head on non-conformance.";
                            }
                            if (token.SelectToken("decision").ToString().Equals("Forward to Closer"))
                            {

                                _context.QAFlows.Add(new QAFlow
                                {
                                    ObservationId = observation_id,
                                    TableName = "qa_officer",
                                    FromId = loggedUserId,
                                    ToId = observation.QaOfficerId,
                                    Completed = "N",
                                    Date = DateTime.Now
                                });
                                observation.Status = "With QA Officer";
                                status = "With QA Officer";
                                mailfrom = user_id;
                                mailto = observation.QaOfficerId;
                                subject = "Forward by Business Head for closer on non-conformance.";
                            }
                            sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                        }

                        foreach (String att1 in att)
                        {
                            if (att1 == null)
                            {
                                continue;
                            }
                            sqlhelp.insert("qa_att", att1.ToString(), "flow", flow_id);
                        }


                    }
                    else if (observation.IsCritical)
                    {
                        if (token.SelectToken("assigned_to") != null)
                        {
                            int assigned_to = int.Parse(token.SelectToken("assigned_to").ToString());
                            _context.BusinessHeads.Add(new BusinessHead
                            {
                                ObservationId = observation_id,
                                FlowId = currentFlow.Id,
                                Remarks = token.SelectToken("remarks").ToString(),
                                DecisionById = loggedUserId,
                                DecisionDate = DateTime.Now,
                                AssignToId = assigned_to,
                                TargetDate = token.SelectToken("target_date").ToString(),
                            });

                            _context.QAFlows.Add(new QAFlow
                            {
                                ObservationId = observation_id,
                                TableName = "assignee",
                                FromId = loggedUserId,
                                ToId = assigned_to,
                                Completed = "N",
                                Date = DateTime.Now
                            });
                            observation.Status = "With Assignee Section";
                            status = "With Assignee Section";
                            sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                            currentFlow.Completed = "Y";
                            mailfrom = user_id;
                            mailto = assigned_to;
                        }
                        else
                        {
                            _context.BusinessHeads.Add(new BusinessHead
                            {
                                ObservationId = observation_id,
                                FlowId = currentFlow.Id,
                                Remarks = token.SelectToken("remarks").ToString(),
                                DecisionById = loggedUserId,
                                DecisionDate = DateTime.Now,
                                Decision = token.SelectToken("decision").ToString()
                            });

                            if (token.SelectToken("decision").ToString().Equals("Return to assignee"))
                            {
                                int assignee_id = currentFlow.FromId.Value;

                                _context.QAFlows.Add(new QAFlow
                                {
                                    ObservationId = observation_id,
                                    TableName = "assignee",
                                    FromId = loggedUserId,
                                    ToId = assignee_id,
                                    Completed = "N",
                                    Date = DateTime.Now
                                });
                                observation.Status = "With Assignee Section";

                                status = "With Assignee Section";
                                mailfrom = user_id;
                                mailto = assignee_id;
                                subject = "Escalated by QA officer on non-conformance.";
                            }

                            if (token.SelectToken("decision").ToString().Equals("Forward to Closer"))
                            {
                                _context.QAFlows.Add(new QAFlow
                                {
                                    ObservationId = observation_id,
                                    TableName = "head_detp",
                                    FromId = int.Parse(user_id),
                                    ToId = observation.HeadDetpId,
                                    Completed = "N",
                                    Date = DateTime.Now
                                });
                                observation.Status = "With Head DETP";
                                status = "With Head DETP";
                                mailfrom = user_id;
                                mailto = int.Parse(to);
                                subject = "Input given by Business Head on non-conformance.";
                            }
                            currentFlow.Completed = "Y";
                            _context.SaveChanges();
                        }
                    }

                    foreach (String att1 in att)
                    {
                        if (att1 == null)
                        {
                            continue;
                        }
                        sqlhelp.insert("qa_att", att1.ToString(), "flow", flow_id);
                    }
                }

                else if (GetRoleByObservation(observation_id) == RoleConst.ASSIGNEE_SECTION && observation.IsCritical)
                {
                    JToken token = JObject.Parse(data);

                    if (!IsJobStopped(observation_id))
                    {

                        sqlhelp.insert("assignee", observation_id.ToString(), flow_id, token.SelectToken("observation_details").ToString(), token.SelectToken("root_cause_analysis").ToString(), token.SelectToken("corrective_action").ToString(), token.SelectToken("preventive_action").ToString(), token.SelectToken("value_of_rectification").ToString(), token.SelectToken("time_loss").ToString(), token.SelectToken("time_value").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());


                        sqlhelp.insert("qa_flow", observation_id.ToString(), "business_head", user_id, to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");

                        sqlhelp.performAction("update qa_observation set status = 'With Business Head' where serial_no = '" + observation_id + "'");
                        status = "With Business Head";

                    }
                    else
                    {
                        sqlhelp.fetch1($"SELECT TOP 1 dept_hod FROM qa_observation WHERE serial_no={observation_id}");

                        sqlhelp.insert("assignee", observation_id.ToString(), flow_id, token.SelectToken("observation_details").ToString(), token.SelectToken("root_cause_analysis").ToString(), token.SelectToken("corrective_action").ToString(), token.SelectToken("preventive_action").ToString(), token.SelectToken("value_of_rectification").ToString(), token.SelectToken("time_loss").ToString(), token.SelectToken("time_value").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                        sqlhelp.insert("qa_flow", observation_id.ToString(), "dept_hod", user_id, sqlhelp.datatable1.Rows[0].ItemArray[0].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                        sqlhelp.performAction("update qa_observation set status = 'With Dept HOD' where serial_no = '" + observation_id + "'");
                        status = "With Dept HOD";
                    }
                    sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");

                    foreach (String att1 in att)
                    {
                        if (att1 == null)
                        {
                            continue;
                        }
                        sqlhelp.insert("qa_att", att1.ToString(), "flow", flow_id);
                    }
                    mailfrom = user_id;
                    mailto = int.Parse(to);
                    subject = "Input given by  Assignee on non-conformance.";
                }

                else if (GetRoleByObservation(observation_id) == RoleConst.ASSIGNEE_SECTION && !observation.IsCritical)
                {
                    JToken token = JObject.Parse(data);
                    sqlhelp.insert("assignee", observation_id.ToString(), flow_id, token.SelectToken("observation_details").ToString(), token.SelectToken("root_cause_analysis").ToString(), token.SelectToken("corrective_action").ToString(), token.SelectToken("preventive_action").ToString(), token.SelectToken("value_of_rectification").ToString(), token.SelectToken("time_loss").ToString(), token.SelectToken("time_value").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());



                    sqlhelp.insert("qa_flow", observation_id.ToString(), "business_head", user_id, to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");

                    sqlhelp.performAction("update qa_observation set status = 'With Business Head' where serial_no = '" + observation_id + "'");
                    status = "With Business Head";

                    sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                    if (att != null)
                    {
                        foreach (String att1 in att)
                        {
                            if (att1 != null)
                            {
                                sqlhelp.insert("qa_att", att1.ToString(), "flow", flow_id);
                            }
                        }
                    }
                    mailfrom = user_id;
                    mailto = int.Parse(to);
                    subject = "Input given by  Assignee on non-conformance.";
                }




                else if (GetRoleByObservation(observation_id) == RoleConst.SITE_INCHARGE)
                {
                    // if yes then to QA officer
                    // if no then to project incharge
                    JToken token = JObject.Parse(data);

                    _context.SiteIncharges.Add(new SiteIncharge
                    {
                        ObservationId = observation_id,
                        FlowId = int.Parse(flow_id),
                        ValueRec = token.SelectToken("valuerec").ToString(),
                        TimeLoss = token.SelectToken("timeloss").ToString(),
                        TimeVal = token.SelectToken("timeval").ToString(),
                        Remarks = token.SelectToken("remarks").ToString(),
                        DecisionById = loggedUserId,
                        DecisionDate = DateTime.Now,
                    });


                    if (!IsJobStopped(observation_id))
                    {
                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = observation_id,
                            TableName = "qa_officer",
                            FromId = loggedUserId,
                            ToId = int.Parse(to),
                            Date = DateTime.Now,
                            Completed = "N"
                        });
                        observation.Status = "With QA Officer";
                        status = "With QA Officer";
                    }
                    else
                    {
                        int submit_to = observation.DeptHodId.Value;
                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = observation_id,
                            TableName = "dept_hod",
                            FromId = loggedUserId,
                            ToId = submit_to,
                            Date = DateTime.Now,
                            Completed = "N"
                        });

                        observation.Status = "With Dept HOD";

                        status = "With Dept HOD";
                    }

                    if (att != null)
                    {
                        foreach (string att1 in att)
                        {
                            if (att1 != null)
                            {
                                _context.QaAtts.Add(new QaAtt
                                {
                                    Data = att1,
                                    Type = "flow",
                                    TypeId = int.Parse(flow_id)
                                });
                                //sqlhelp.insert("qa_att", att1.ToString(), "flow", flow_id);
                            }
                        }
                    }

                    currentFlow.Completed = "Y";
                    mailfrom = user_id;
                    mailto = int.Parse(to);
                    subject = "Input given by site in charge on non-conformance.";
                    _context.SaveChanges();
                }

                else if (GetRoleByObservation(observation_id) == RoleConst.PROJECT_INCHARGE)
                {

                    JToken token = JObject.Parse(data);

                    _context.ProjectIncharges.Add(new ProjectIncharge
                    {
                        ObservationId = observation_id,
                        FlowId = currentFlow.Id,
                        ValueRec = token.SelectToken("valuerec").ToString(),
                        TimeLoss = token.SelectToken("timeloss").ToString(),
                        TimeVal = token.SelectToken("timeval").ToString(),
                        Remarks = token.SelectToken("remarks").ToString(),
                        WithinTargetDate = "",
                        DecisionById = loggedUserId,
                        DecisionDate = DateTime.Now,
                    });

                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = observation_id,
                        TableName = "qa_officer",
                        FromId = loggedUserId,
                        ToId = int.Parse(to),
                        Date = DateTime.Now,
                        Completed = "N"
                    });

                    observation.Status = "With QA Officer";
                    status = "With QA Officer";
                    currentFlow.Completed = "Y";
                    mailfrom = user_id;
                    mailto = int.Parse(to);
                    subject = "Input given by Project in charge on non-conformance.";

                    if (att != null)
                    {
                        foreach (String att1 in att)
                        {
                            if (att1 != null)
                            {
                                _context.QaAtts.Add(new QaAtt
                                {
                                    Data = att1,
                                    Type = "flow",
                                    TypeId = int.Parse(flow_id)
                                });
                            }
                        }
                    }

                }

                else if (GetRoleByObservation(observation_id) == RoleConst.DEPARTMENT_HOD)
                {
                    JToken token = JObject.Parse(data);
                    if (!IsJobStopped(observation_id))
                    {

                        sqlhelp.insert("dept_hod", observation_id.ToString(), flow_id, token.SelectToken("valuerec").ToString(), token.SelectToken("timeloss").ToString(), token.SelectToken("timeval").ToString(), "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString(), "");

                        sqlhelp.insert("qa_flow", observation_id.ToString(), "qa_officer", user_id, to, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                        sqlhelp.performAction("update qa_observation set status = 'With QA Officer' where serial_no = '" + observation_id + "'");
                        status = "With QA Officer";
                        sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");

                    }

                    else
                    {
                        if (from != RoleConst.HEAD_DETP.ToString() && from != RoleConst.QA_OFFICER.ToString() && from != RoleConst.HOD_QA_SHA.ToString() && from != RoleConst.ASSIGNEE_SECTION.ToString())
                        {
                            sqlhelp.fetch1("select site_incharge from qa_observation where serial_no = " + observation_id);
                            string siteIncharge = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();

                            sqlhelp.insert("dept_hod", observation_id.ToString(), flow_id, token.SelectToken("valuerec").ToString(), token.SelectToken("timeloss").ToString(), token.SelectToken("timeval").ToString(), "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString(), "");

                            sqlhelp.insert("qa_flow", observation_id.ToString(), "site_incharge", user_id, siteIncharge, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                            sqlhelp.performAction("update qa_observation set status = 'With Site Incharge' where serial_no = '" + observation_id + "'");
                            status = "With Site Incharge";
                        }

                        else if (from == RoleConst.ASSIGNEE_SECTION.ToString())
                        {
                            if (token.SelectToken("decision").ToString() == "Not Satisfied")
                            {

                                sqlhelp.fetch1($"SELECT TOP 1 to_id FROM qa_flow WHERE observation_id={observation_id} AND table_name='assignee' ORDER BY id DESC");
                                var assignee = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();


                                sqlhelp.insert("dept_hod", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), "", "", "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString(), assignee);

                                sqlhelp.insert("qa_flow", observation_id.ToString(), "assignee", user_id, assignee, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                                sqlhelp.performAction("update qa_observation set status = 'With Assignee Section' where serial_no = '" + observation_id + "'");
                                status = "With Assignee Section";
                            }
                            else
                            {
                                // TODO: review
                                sqlhelp.fetch1("select head_detp from qa_observation where serial_no = " + observation_id);

                                string toSite = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                                sqlhelp.insert("dept_hod", observation_id.ToString(), flow_id, token.SelectToken("decision").ToString(), "", "", "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString(), "");
                                // TODO: review
                                sqlhelp.insert("qa_flow", observation_id.ToString(), "head_detp", user_id, toSite, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                                sqlhelp.performAction("update qa_observation set status = 'With Head DETP' where serial_no = '" + observation_id + "'");
                                status = "With Head DETP";
                            }
                        }

                        else if (from == RoleConst.QA_OFFICER.ToString() || from == RoleConst.HOD_QA_SHA.ToString() || from == RoleConst.HEAD_DETP.ToString())
                        {

                            var assignedTo = token.SelectToken("assigned_to").ToString();

                            sqlhelp.insert("dept_hod", observation_id.ToString(), flow_id, null, "", "", "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString(), token.SelectToken("assigned_to").ToString());

                            sqlhelp.insert("qa_flow", observation_id.ToString(), "assignee", user_id, assignedTo, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                            sqlhelp.performAction("update qa_observation set status = 'With Assignee Section' where serial_no = '" + observation_id + "'");

                            status = "With Assignee Section";
                        }

                        sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                    }
                    if (att != null)
                    {

                        foreach (String att1 in att)
                        {
                            if (att1 != null)
                            {
                                sqlhelp.insert("qa_att", att1.ToString(), "flow", flow_id);
                            }
                        }
                    }

                    mailfrom = user_id;
                    mailto = int.Parse(to);
                    subject = "Input given by Department HOD on non-conformance.";
                }

                else if (GetRoleByObservation(observation_id) == RoleConst.QA_OFFICER && !observation.IsCritical)
                {
                    // if yes then close
                    // if no then to Dept HOD

                    JToken token = JObject.Parse(data);
                    _context.QAOfficers.Add(new QAOfficer
                    {
                        ObservationId = observation_id,
                        FlowId = currentFlow.Id,
                        ComplianceSatisfactory = token.SelectToken("compliance_satisfactory").ToString(),
                        Remarks = token.SelectToken("remarks").ToString(),
                        DecisionById = loggedUserId,
                        DecisionDate = DateTime.Now,
                        WithinSlg = within_slg,
                    });

                    if (token.SelectToken("compliance_satisfactory").ToString().Equals("Yes"))
                    {
                        sqlhelp.performAction("update qa_observation set status = 'Closed by QA Officer' where serial_no = '" + observation_id + "'");
                        status = "Closed by QA Officer";
                        subject = "Non -conformance closed by QA Officer.";
                    }

                    else
                    {
                        // fetching to id
                        sqlhelp.fetch1("Select TOP 1 table_name,id from qa_flow where observation_id = '" + observation_id + "' AND id NOT IN ('" + flow_id + "') ORDER BY id DESC");
                        String flowtblname = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();

                        String flow2id = sqlhelp.datatable1.Rows[0].ItemArray[1].ToString();
                        sqlhelp.fetch1("Select TOP 1 table_name from qa_flow where observation_id = '" + observation_id + "' AND id NOT IN ('" + flow_id + "','" + flow2id + "') ORDER BY id DESC");
                        String flow2tblname = "";
                        if (sqlhelp.datatable1.Rows.Count > 0)
                        {
                            flow2tblname = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                        }

                        var totblname = "";
                        var submit_to2 = "";
                        var st2 = "";
                        if (flowtblname.Equals("site_incharge"))
                        {
                            totblname = "project_incharge";
                            st2 = "With Project Incharge";
                        }

                        else if (flowtblname.Equals("project_incharge"))
                        {
                            totblname = "dept_hod";
                            st2 = "With Dept HOD";
                        }

                        else if (flowtblname.Equals("dept_hod"))
                        {
                            totblname = "business_head";
                            st2 = "With Business Head";
                        }

                        else if (flowtblname.Equals("business_head") && !flow2tblname.Equals("assignee"))
                        {
                            totblname = "assignee";
                            st2 = "With Assignee Section";
                        }

                        else if (flowtblname.Equals("business_head") && flow2tblname.Equals("assignee"))
                        {
                            totblname = "business_head";
                            st2 = "With Business Head";
                        }

                        else if (flowtblname.Equals("assignee"))
                        {
                            totblname = "business_head";
                            st2 = "With Business Head";
                        }
                        status = st2;
                        if (!totblname.Equals("assignee"))
                        {
                            sqlhelp.fetch1("Select " + totblname + " from qa_observation where serial_no = '" + observation_id + "'");
                            submit_to2 = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                        }
                        else
                        {
                            //assignee
                        }

                        sqlhelp.insert("qa_flow", observation_id.ToString(), totblname, user_id, submit_to2, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                        sqlhelp.performAction("update qa_observation set status = '" + st2 + "' where serial_no = '" + observation_id + "'");
                        mailfrom = user_id;
                        mailto = int.Parse(submit_to2);
                        subject = "Escalated by QA officer on non-conformance.";

                    }

                    if (att != null)
                    {
                        foreach (String att1 in att)
                        {
                            if (att1 != null)
                            {
                                sqlhelp.insert("qa_att", att1.ToString(), "flow", flow_id);
                            }
                        }
                    }
                    sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");

                }
                else if (GetRoleByObservation(observation_id) == RoleConst.QA_OFFICER && observation.IsCritical)
                {
                    // if yes then close
                    // if no then to Dept HOD

                    JToken token = JObject.Parse(data);
                    sqlhelp.insert("qa_officer", observation_id.ToString(), flow_id, token.SelectToken("compliance_satisfactory").ToString(), token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString(), within_slg);

                    if (token.SelectToken("compliance_satisfactory").ToString().Equals("Yes"))
                    {
                        sqlhelp.performAction("update qa_observation set status = 'Closed by QA Officer' where serial_no = '" + observation_id + "'");
                        status = "Closed by QA Officer";
                        subject = "Non -conformance closed by QA Officer.";
                    }
                    else
                    {
                        sqlhelp.fetch1("Select head_detp from qa_observation where serial_no = '" + observation_id + "'");
                        var submit_to3 = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                        sqlhelp.insert("qa_flow", observation_id.ToString(), "head_detp", user_id, submit_to3, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "N");
                        sqlhelp.performAction("update qa_observation set status = 'With Head Detp' where serial_no = '" + observation_id + "'");
                        status = "With HoD QA & SHA";
                        mailfrom = user_id;
                        mailto = int.Parse(submit_to3);
                        subject = "Escalated by QA officer on non-conformance.";
                    }
                    sqlhelp.performAction("update qa_flow set completed = 'Y' where id = '" + flow_id + "'");
                    if (att != null)
                    {
                        foreach (String att1 in att)
                        {
                            if (att1 != null)
                            {
                                sqlhelp.insert("qa_att", att1.ToString(), "flow", flow_id);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
                return Json(new { status = false, message = e });
            }

            if (mailto != 0)
            {
                var user = _context.Users.Where(x => x.UserId == mailto).FirstOrDefault();
                String email_id = user.Email;
                String name = user.Name;
                String body = $"Dear Sir/Mam,   <br />&nbsp;&nbsp;&nbsp; Please Click here to see the details-->> <b><a href='{_configuration["SiteDetails:Production_URL"]}/EditObservation?serial_no=" + observation_id + "&user_id=" + mailto + "' target='_blank'>VIEW OBSERVATION</a></b><br><br>Thanks,<br>QA- DETP<br>Tata Steel UISL";
                mail.SendMail(email_id, subject, body);
            }

            if (status.Contains("Head DETP"))
            {
                status = "With Section Head QA";
            }
            if (status.Contains("EIC DETP"))
            {
                status = status.Replace("EIC DETP", "HoD DETP");
            }
            _context.SaveChanges();
            return Json(new { status = true, message = "success", result = status });
        }

        public bool IsJobStopped(int serial_no)
        {
            var observation = _context.QAObservations.Where(x => x.SerialNo == serial_no).FirstOrDefault();
            return observation.Job == "Yes";
        }

        public int GetRoleByObservation(int observationId)
        {
            var qaObservation = _context.QAObservations.Where(x => x.SerialNo == observationId).FirstOrDefault();
            string status = qaObservation.Status;

            switch (status)
            {
                case "With Assignee Section":
                    return RoleConst.ASSIGNEE_SECTION;
                case "With Business Head":
                    return RoleConst.BUSINESS_HEAD;
                case "With QA Officer":
                    return RoleConst.QA_OFFICER;
                case "With Site Incharge":
                    return RoleConst.SITE_INCHARGE;
                case "With Head DETP":
                    return RoleConst.HEAD_DETP;
                case "With EIC DETP":
                    return RoleConst.EIC_DETP;
                case "With Project Incharge":
                    return RoleConst.PROJECT_INCHARGE;
                case "With Dept HOD":
                    return RoleConst.DEPARTMENT_HOD;
                case "With HoD QA & SHA":
                    return RoleConst.HOD_QA_SHA;
                default:
                    break;
            }

            return 0;

        }

        public List<QAFlow> GetQAFlow(String serial_no)
        {
            return _context.QAFlows.Include(x => x.To).Where(x => x.ObservationId.ToString() == serial_no).ToList();
        }

        public List<QaAtt> GetQAAtt(int serial_no)
        {
            return _context.QaAtts.Where(x => x.Type == "observation" && x.TypeId == serial_no).ToList();
        }

        public List<QaAtt> GetQAAttFlow(int flow_id)
        {
            return _context.QaAtts.Where(x => x.Type == "flow" && x.TypeId == flow_id).ToList();
        }




        private bool GetJobStoped(string serial_no)
        {
            sqlhelp.fetch1("Select job from qa_observation where serial_no = '" + serial_no + "'");
            return sqlhelp.datatable1.Rows[0].ItemArray[0].ToString() == "Yes";
        }

        public String checkAssignee(int serial_no)
        {
            sqlhelp.fetch1("Select * from qa_flow where id = '" + serial_no + "'");
            return (sqlhelp.datatable1.Rows[0].ItemArray[3].ToString());
        }

    }
}
