using DETP.data;
using DETP.model;
using DETP.requests.editObservation;
using System.Linq;
using System;
using Newtonsoft.Json.Linq;
using DETP.Constant;
using System.Xml.Linq;
using System.Web;
using DETP.Controllers;
using DETP.helpers;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

namespace DETP.services
{
    public class ObservationFlowService : BaseService
    {
        private readonly ApplicationDbContext _context;
        public ObservationFlowService(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public string SaveSiteIncharge(SiteInchargeCreateRequest request, int userId)
        {
            var observation = _context.QAObservations.Where(x => x.SerialNo == request.ObservationId).FirstOrDefault();


            _context.SiteIncharges.Add(new SiteIncharge
            {
                ObservationId = request.ObservationId,
                FlowId = request.FlowId,
                ValueRec = request.ValueRec.ToString(),
                TimeLoss = request.TimeLoss.ToString(),
                TimeVal = request.TimeVal.ToString(),
                Remarks = request.Remarks.ToString(),
                DecisionById = userId,
                DecisionDate = DateTime.Now,
            });

            _context.QAFlows.Where(x => x.Id == request.FlowId).FirstOrDefault().Completed = "Y";
            if (!observation.IsJobStopped)
            {
                _context.QAFlows.Add(new QAFlow
                {
                    ObservationId = request.ObservationId,
                    TableName = "qa_officer",
                    FromId = userId,
                    ToId = request.ToId,
                    Date = DateTime.Now,
                    Completed = "N"
                });
                observation.Status = "With QA Officer";
            }
            else
            {
                int submit_to = observation.DeptHodId.Value;
                _context.QAFlows.Add(new QAFlow
                {
                    ObservationId = request.ObservationId,
                    TableName = "dept_hod",
                    FromId = userId,
                    ToId = submit_to,
                    Date = DateTime.Now,
                    Completed = "N"
                });

                observation.Status = "With Dept HOD";
            }

            if (request.Att != null)
            {
                SaveFlowAtt(request.Att, request.FlowId);
            }

            _context.SaveChanges();

            return observation.Status;
        }

        public string SaveProjectIncharge(ProjectInchargeCreateRequest request, int userId)
        {
            var observation = _context.QAObservations.Where(x => x.SerialNo == request.ObservationId).FirstOrDefault();
            var currentFlow = _context.QAFlows.Where(x => x.Id == request.FlowId).FirstOrDefault();


            _context.ProjectIncharges.Add(new ProjectIncharge
            {
                ObservationId = request.ObservationId,
                FlowId = request.FlowId,
                ValueRec = request.ValueRec,
                TimeLoss = request.TimeLoss,
                TimeVal = request.TimeVal,
                Remarks = request.Remarks,
                WithinTargetDate = "",
                DecisionById = userId,
                DecisionDate = DateTime.Now,
            });

            _context.QAFlows.Add(new QAFlow
            {
                ObservationId = request.ObservationId,
                TableName = "qa_officer",
                FromId = userId,
                ToId = observation.QaOfficerId,
                Date = DateTime.Now,
                Completed = "N"
            });

            observation.Status = "With QA Officer";
            string status = "With QA Officer";
            currentFlow.Completed = "Y";
            int mailfrom = userId;
            int? mailto = observation.QaOfficerId;
            string subject = "Input given by Project in charge on non-conformance.";

            this.SaveFlowAtt(request.Att, request.FlowId);
            _context.SaveChanges();

            SendObservationUpdatedMail(request.ObservationId, subject, mailto.Value);

            return status;
        }

        public string SaveQAOfficer(QAOfficerCreateRequest request, int userId)
        {
            var observation = _context.QAObservations.Where(x => x.SerialNo == request.ObservationId).FirstOrDefault();
            var currentFlow = _context.QAFlows.Where(x => x.Id == request.FlowId).FirstOrDefault();
            var status = "";
            var subject = "";
            var mailfrom = 0;
            int? mailto = 0;

            _context.QAOfficers.Add(new QAOfficer
            {
                ObservationId = request.ObservationId,
                FlowId = request.FlowId,
                ComplianceSatisfactory = request.ComplianceSatisfactory,
                Remarks = request.Remarks,
                DecisionById = userId,
                DecisionDate = DateTime.Now,
                WithinSlg = request.WithinSlg,
            });

            if (request.ComplianceSatisfactory.Equals("Yes"))
            {
                observation.Status = "Closed by QA Officer";

                status = "Closed by QA Officer";
                subject = "Non-conformance closed by QA Officer.";
            }

            if (!observation.IsCritical)
            {




                if (!request.ComplianceSatisfactory.Equals("Yes"))
                {

                    var flowTable = _context.QAFlows.Where(x => x.Id != currentFlow.Id).OrderByDescending(x => x.Id).FirstOrDefault();

                    var flow2Table = _context.QAFlows.Where(x => x.Id != currentFlow.Id && x.Id != flowTable.Id).OrderByDescending(x => x.Id).FirstOrDefault();


                    var totblname = "";
                    int? submit_to2 = null;
                    var st2 = "";

                    if (flowTable.TableName.Equals("site_incharge"))
                    {
                        totblname = "project_incharge";
                        st2 = "With Project Incharge";
                        submit_to2 = observation.ProjectInchargeId;
                    }

                    else if (flowTable.TableName.Equals("project_incharge"))
                    {
                        totblname = "dept_hod";
                        st2 = "With Dept HOD";
                        submit_to2 = observation.DeptHodId;
                    }

                    else if (flowTable.TableName.Equals("dept_hod"))
                    {
                        totblname = "business_head";
                        st2 = "With Business Head";
                        submit_to2 = observation.BusinessHeadId;
                    }

                    else if (flowTable.TableName.Equals("business_head") && !flow2Table.TableName.Equals("assignee"))
                    {
                        totblname = "assignee";
                        st2 = "With Assignee Section";
                    }

                    else if (flowTable.TableName.Equals("business_head") && flow2Table.TableName.Equals("assignee"))
                    {
                        totblname = "business_head";
                        st2 = "With Business Head";
                        submit_to2 = observation.BusinessHeadId;
                    }

                    else if (flowTable.TableName.Equals("assignee"))
                    {
                        totblname = "business_head";
                        st2 = "With Business Head";
                        submit_to2 = observation.BusinessHeadId;
                    }
                    status = st2;

                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = totblname,
                        FromId = userId,
                        ToId = submit_to2.Value,
                        Date = DateTime.Now,
                        Completed = "N"
                    });

                    observation.Status = st2;

                    mailfrom = userId;
                    mailto = submit_to2;
                    subject = "Escalated by QA officer on non-conformance.";

                }

                currentFlow.Completed = "Y";

            }
            else if (observation.IsCritical)
            {


                if (!request.ComplianceSatisfactory.Equals("Yes"))
                {

                    var submit_to3 = observation.HeadDetpId;
                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = "head_detp",
                        FromId = userId,
                        ToId = submit_to3,
                        Date = DateTime.Now,
                        Completed = "N"
                    });
                    observation.Status = "With Head Detp";

                    status = "With HoD QA & SHA";
                    mailfrom = userId;
                    mailto = submit_to3;
                    subject = "Escalated by QA officer on non-conformance.";
                }
                currentFlow.Completed = "Y";

            }

            this.SaveFlowAtt(request.Att, request.FlowId);
            _context.SaveChanges();

            SendObservationUpdatedMail(request.ObservationId, subject, mailto.Value);

            return status;
        }

        public string SaveDeptHod(DeptHodCreateRequest request, int userId)
        {
            var observation = _context.QAObservations.Where(x => x.SerialNo == request.ObservationId).FirstOrDefault();
            var currentFlow = _context.QAFlows.Where(x => x.Id == request.FlowId).FirstOrDefault();

            string status = "";
            string subject = "";
            int? to = null;


            if (!observation.IsJobStopped)
            {
                _context.DeptHods.Add(new DeptHod
                {
                    ObservationId = request.ObservationId,
                    FlowId = request.FlowId,
                    ValueRec = request.ValueRec,
                    TimeLoss = request.TimeLoss,
                    TimeVal = request.TimeVal,
                    Remarks = request.Remarks,
                    DecisionById = userId,
                    DecisionDate = DateTime.Now,
                });
                

                _context.QAFlows.Add(new QAFlow
                {
                    ObservationId = request.ObservationId,
                    TableName = "qa_officer",
                    FromId = userId,
                    ToId = observation.QaOfficerId,
                    Date = DateTime.Now,
                    Completed = "N"
                });
                
                observation.Status = "With QA Officer";
                
                
                currentFlow.Completed = "Y";
                to = observation.QaOfficerId;
                

            }

            else
            {
                if (request.FromId != RoleConst.HEAD_DETP && request.FromId != RoleConst.QA_OFFICER && request.FromId != RoleConst.HOD_QA_SHA && request.FromId != RoleConst.ASSIGNEE_SECTION)
                {
                    _context.DeptHods.Add(new DeptHod
                    {
                        ObservationId = request.ObservationId,
                        FlowId = request.FlowId,
                        ValueRec = request.Decision,
                        Remarks = request.Remarks,
                        DecisionById = userId,
                        DecisionDate = DateTime.Now,
                    });
                    
                    

                    
                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = "site_incharge",
                        FromId = userId,
                        ToId = observation.SiteInchargeId,
                        Date = DateTime.Now,
                        Completed = "N"
                    });
                    
                    observation.Status = "With Site Incharge";
                    
                    status = "With Site Incharge";
                    to = observation.SiteInchargeId;
                }

                else if (request.FromId == RoleConst.ASSIGNEE_SECTION)
                {
                    if (request.Decision == "Not Satisfied")
                    {
                        
                        
                        

                        _context.DeptHods.Add(new DeptHod
                        {
                            ObservationId = request.ObservationId,
                            FlowId = request.FlowId,
                            ValueRec = request.Decision,
                            Remarks = request.Remarks,
                            DecisionById = userId,
                            DecisionDate = DateTime.Now,
                            AssignToId = currentFlow.ToId ?? 0,
                        });
                        
                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = request.ObservationId,
                            TableName = "assignee",
                            FromId = userId,
                            ToId = currentFlow.ToId ?? 0,
                            Date = DateTime.Now,
                            Completed = "N"
                        });

                        
                        observation.Status = "With Assignee Section";
                        
                        status = "With Assignee Section";
                        to = currentFlow.ToId;
                    }
                    else
                    {
                        
                        

                        
                        _context.DeptHods.Add(new DeptHod
                        {
                            ObservationId = request.ObservationId,
                            FlowId = request.FlowId,
                            ValueRec = request.Decision,
                            Remarks = request.Remarks,
                            DecisionById = userId,
                            DecisionDate = DateTime.Now,
                        });
                        
                        
                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = request.ObservationId,
                            TableName = "head_detp",
                            FromId = userId,
                            ToId = observation.HeadDetpId,
                            Date = DateTime.Now,
                            Completed = "N"
                        });
                        
                        observation.Status = "With Head DETP";
                        
                        status = "With Head DETP";
                        to = observation.HeadDetpId;
                    }
                }

                else if (request.FromId == RoleConst.QA_OFFICER || request.FromId == RoleConst.HOD_QA_SHA || request.FromId == RoleConst.HEAD_DETP)
                {

                    var assignedTo = request.AssignedToId;
                    _context.DeptHods.Add(new DeptHod
                    {
                        ObservationId = request.ObservationId,
                        FlowId = request.FlowId,
                        Remarks = request.Remarks,
                        DecisionById = userId,
                        DecisionDate = DateTime.Now,
                        AssignToId = assignedTo.Value,
                    });

                    

                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = "assignee",
                        FromId = userId,
                        ToId = assignedTo,
                        Date = DateTime.Now,
                        Completed = "N"
                    });

                    
                    observation.Status = "With Assignee Section";
                    

                    status = "With Assignee Section";
                    to = assignedTo;
                }
                currentFlow.Completed = "Y";
                
            }
            this.SaveFlowAtt(request.Att, request.FlowId);
            _context.SaveChanges();

            int mailfrom = userId;
            int? mailto = to;
            subject = "Input given by Department HOD on non-conformance.";

            SendObservationUpdatedMail(request.ObservationId, subject, mailto.Value);

            return observation.Status;
        }

        public string SaveBusinessHead(BusinessHeadCreateRequest request, int userId)
        {
            var observation = _context.QAObservations.Where(x => x.SerialNo == request.ObservationId).FirstOrDefault();
            var currentFlow = _context.QAFlows.Where(x => x.Id == request.FlowId).FirstOrDefault();

            var status = "";
            var subject = "";
            var mailfrom = userId;
            int? mailto = 0;

            if (!observation.IsCritical)
            {
                if (request.AssignedTo != null)
                {

                    _context.BusinessHeads.Add(new BusinessHead
                    {
                        ObservationId = request.ObservationId,
                        FlowId = request.FlowId,
                        AssignToId = request.AssignedTo,
                        TargetDate = request.TargetDate,
                        Remarks = request.Remarks,
                        DecisionById = userId,
                        DecisionDate = DateTime.Now
                    });

                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = "assignee",
                        FromId = userId,
                        ToId = request.AssignedTo,
                        Completed = "N",
                        Date = DateTime.Now
                    });
                    observation.Status = "With Assignee Section";
                    status = AppConstants.ObservationStatus.WITH_ASSIGNEE_SECTION;
                    currentFlow.Completed = "Y";
                    mailto = request.AssignedTo;
                    subject = "Input given by Business Head on non-conformance.";
                }
                else
                {
                    _context.BusinessHeads.Add(new BusinessHead
                    {
                        ObservationId = request.ObservationId,
                        FlowId = currentFlow.Id,
                        Remarks = request.Remarks,
                        DecisionById = userId,
                        DecisionDate = DateTime.Now,
                        Decision = request.Decision
                    });
                    if (request.Decision == "Return to assignee")
                    {

                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = request.ObservationId,
                            TableName = "assignee",
                            FromId = userId,
                            ToId = currentFlow.FromId,
                            Completed = "N",
                            Date = DateTime.Now
                        });
                        observation.Status = "With Assignee Section";
                        status = AppConstants.ObservationStatus.WITH_ASSIGNEE_SECTION;

                        mailto = currentFlow.FromId;
                        subject = "Input given by Business Head on non-conformance.";
                    }
                    if (request.Decision == "Forward to Closer")
                    {

                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = request.ObservationId,
                            TableName = "qa_officer",
                            FromId = userId,
                            ToId = observation.QaOfficerId,
                            Completed = "N",
                            Date = DateTime.Now
                        });
                        observation.Status = "With QA Officer";
                        status = "With QA Officer";
                        mailto = observation.QaOfficerId;
                        subject = "Forward by Business Head for closer on non-conformance.";
                    }
                    currentFlow.Completed = "Y";
                }

                this.SaveFlowAtt(request.Att, request.FlowId);


            }
            else if (observation.IsCritical)
            {
                if (request.AssignedTo != null)
                {

                    _context.BusinessHeads.Add(new BusinessHead
                    {
                        ObservationId = request.ObservationId,
                        FlowId = currentFlow.Id,
                        Remarks = request.Remarks,
                        DecisionById = userId,
                        DecisionDate = DateTime.Now,
                        AssignToId = request.AssignedTo,
                        TargetDate = request.TargetDate,
                    });

                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = "assignee",
                        FromId = userId,
                        ToId = request.AssignedTo,
                        Completed = "N",
                        Date = DateTime.Now
                    });
                    observation.Status = "With Assignee Section";
                    status = "With Assignee Section";
                    currentFlow.Completed = "Y";
                    mailto = request.AssignedTo;
                }
                else
                {
                    _context.BusinessHeads.Add(new BusinessHead
                    {
                        ObservationId = request.ObservationId,
                        FlowId = currentFlow.Id,
                        Remarks = request.Remarks,
                        DecisionById = userId,
                        DecisionDate = DateTime.Now,
                        Decision = request.Decision
                    });

                    if (request.Decision == "Return to assignee")
                    {
                        int assignee_id = currentFlow.FromId.Value;

                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = request.ObservationId,
                            TableName = "assignee",
                            FromId = userId,
                            ToId = assignee_id,
                            Completed = "N",
                            Date = DateTime.Now
                        });
                        observation.Status = "With Assignee Section";

                        status = "With Assignee Section";
                        mailfrom = userId;
                        mailto = assignee_id;
                        subject = "Escalated by QA officer on non-conformance.";
                    }

                    if (request.Decision == "Forward to Closer")
                    {
                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = request.ObservationId,
                            TableName = "head_detp",
                            FromId = userId,
                            ToId = observation.HeadDetpId,
                            Completed = "N",
                            Date = DateTime.Now
                        });
                        observation.Status = "With Head DETP";
                        status = "With Head DETP";
                        mailto = observation.HeadDetpId;
                        subject = "Input given by Business Head on non-conformance.";
                    }
                    currentFlow.Completed = "Y";
                    _context.SaveChanges();
                }
            }

            this.SaveFlowAtt(request.Att, request.FlowId);
            _context.SaveChanges();

            SendObservationUpdatedMail(request.ObservationId, subject, mailto.Value);

            return status;
        }

        public string SaveAssignee(AssigneeCreateRequest request, int userId)
        {
            var observation = _context.QAObservations.Where(x => x.SerialNo == request.ObservationId).FirstOrDefault();
            var currentFlow = _context.QAFlows.Where(x => x.Id == request.FlowId).FirstOrDefault();

            string status = "";
            int? mailTo = null;

            _context.Assignees.Add(new Assignee
            {
                ObservationId = request.ObservationId,
                FlowId = request.FlowId,
                ObservationDetails = request.ObservationDetails,
                RootCauseAnalysis = request.RootCauseAnalysis,
                CorrectiveAction = request.CorrectiveAction,
                PreventiveAction = request.PreventiveAction,
                ValueOfRectification = request.ValueOfRectification,
                TimeLoss = request.TimeLoss,
                TimeValue = request.TimeValue,
                DecisionById = userId,
                DecisionDate = DateTime.Now,
            });
            if (!observation.IsJobStopped)
            {
                _context.QAFlows.Add(new QAFlow
                {
                    ObservationId = request.ObservationId,
                    TableName = "business_head",
                    FromId = userId,
                    ToId = observation.BusinessHeadId,
                    Date = DateTime.Now,
                    Completed = "N"
                });

                observation.Status = "With Business Head";
                currentFlow.Completed = "Y";

                mailTo = observation.BusinessHeadId;


            }
            else
            {
                _context.QAFlows.Add(new QAFlow
                {
                    ObservationId = request.ObservationId,
                    TableName = "dept_hod",
                    FromId = userId,
                    ToId = observation.DeptHodId,
                    Date = DateTime.Now,
                    Completed = "N"
                });

                observation.Status = "With Dept HOD";
                mailTo = observation.DeptHodId;
            }
            currentFlow.Completed = "Y";
            this.SaveFlowAtt(request.Att, request.FlowId);

            int mailfrom = userId;
            string subject = "Input given by  Assignee on non-conformance.";

            _context.SaveChanges();
            SendObservationUpdatedMail(request.ObservationId, subject, mailTo.Value);

            return observation.Status;
        }

        public string SaveHodQaSha(HodQaShaCreateRequest request, int userid)
        {
            var observation = _context.QAObservations.Where(x => x.SerialNo == request.ObservationId).FirstOrDefault();
            var currentFlow = _context.QAFlows.Where(x => x.Id == request.FlowId).FirstOrDefault();

            string subject = "";
            int? mailto;

            if (!observation.IsJobStopped)
            {
                if (request.Decision == "Close")
                {
                    _context.HodQaSha.Add(new HodQaSha
                    {
                        ObservationId = request.ObservationId,
                        FlowId = request.FlowId,
                        Decision = request.Decision,
                        Remarks = request.Remarks,
                        DecisionById = userid,
                        DecisionDate = DateTime.Now
                    });
                    observation.Status = AppConstants.ObservationStatus.CLOSED_BY_HOD_QA_SHA;
                    subject = "Non-conformance closed by With HoD QA & SHA";
                }
                else if (request.Decision == "Return QA")
                {
                    _context.HodQaSha.Add(new HodQaSha
                    {
                        ObservationId = request.ObservationId,
                        FlowId = request.FlowId,
                        Decision = request.Decision,
                        Remarks = request.Remarks,
                        DecisionById = userid,
                        DecisionDate = DateTime.Now
                    });
                    
                    mailto = observation.HeadDetpId;
                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = "head_detp",
                        FromId = userid,
                        ToId = observation.HeadDetpId,
                        Date = DateTime.Now,
                        Completed = "N"
                    });
                    observation.Status = AppConstants.ObservationStatus.WITH_HOD_QA_SHA;
                    subject = "Non-conformance returned by HoD QA & SHA.";
                }
                else
                {
                    _context.HodQaSha.Add(new HodQaSha
                    {
                        ObservationId = request.ObservationId,
                        FlowId = request.FlowId,
                        Decision = request.Decision,
                        Remarks = request.Remarks,
                        DecisionById = userid,
                        DecisionDate = DateTime.Now
                    });
                    mailto = observation.BusinessHeadId;
                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = "business_head",
                        FromId = userid,
                        ToId = observation.BusinessHeadId,
                        Date = DateTime.Now,
                        Completed = "N"
                    });
                    observation.Status = AppConstants.ObservationStatus.WITH_BUSINESS_HEAD;
                    subject = "Non-conformance returned by HoD QA & SHA.";
                }
                currentFlow.Completed = "Y";
            }
            else
            {
                if (request.Decision == "Close")
                {
                    _context.HodQaSha.Add(new HodQaSha
                    {
                        ObservationId = request.ObservationId,
                        FlowId = request.FlowId,
                        Decision = request.Decision,
                        Remarks = request.Remarks,
                        DecisionById = userid,
                        DecisionDate = DateTime.Now
                    });
                    observation.Status = AppConstants.ObservationStatus.CLOSED_BY_HOD_QA_SHA;
                    subject = "Non -conformance closed by HoD QA & SHA";

                }
                else
                {
                    _context.HodQaSha.Add(new HodQaSha
                    {
                        ObservationId = request.ObservationId,
                        FlowId = request.FlowId,
                        Decision = request.Decision,
                        Remarks = request.Remarks,
                        DecisionById = userid,
                        DecisionDate = DateTime.Now
                    });

                    

                    mailto = observation.DeptHodId;

                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = "dept_hod",
                        FromId = userid,
                        ToId = observation.DeptHodId,
                        Date = DateTime.Now,
                        Completed = "N"
                    });
                    
                    observation.Status = "With Dept HOD";
                    subject = "Non -conformance returned by HoD QA & SHA.";
                }
                currentFlow.Completed = "Y";
            }
            _context.SaveChanges();
            return observation.Status;
        }

        public string SaveHeadDetp(HeadDetpCreateRequest request, int userId)
        {
            var observation = _context.QAObservations.Where(x => x.SerialNo == request.ObservationId).FirstOrDefault();
            var currentFlow = _context.QAFlows.Where(x => x.Id == request.FlowId).FirstOrDefault();
            var status = "";
            var subject = "";
            var mailto = 0;

            if (!observation.IsJobStopped)
            {
                if (request.FromId != RoleConst.BUSINESS_HEAD && request.Qaaa != "Yes" && request.FromId != RoleConst.HOD_QA_SHA)
                {

                    if (request.Decision == "Rejected")
                    {
                        _context.HeadDetps.Add(new HeadDetp
                        {
                            ObservationId = request.ObservationId,
                            FlowId = request.FlowId,
                            Decision = request.Decision,
                            Job = request.Job,
                            Remarks = request.Remarks,
                            DecisionById = userId,
                            DecisionDate = DateTime.Now
                        });
                        
                        observation.Status = "Closed by Head DETP";
                        
                        
                        subject = "Non -conformance closed by head DETP.";
                    }
                    else if (request.Decision == "Convert")
                    {
                        _context.HeadDetps.Add(new HeadDetp
                        {
                            ObservationId = request.ObservationId,
                            FlowId = request.FlowId,
                            Decision = request.Decision,
                            Job = request.Job,
                            Remarks = request.Remarks,
                            DecisionById = userId,
                            DecisionDate = DateTime.Now
                        });
                        

                        mailto = observation.SiteInchargeId.Value;
                        
                        observation.Status = "With Site Incharge";
                        observation.NatureOfConfirmance = request.Job;
                        
                        
                        
                        subject = "Input given by QA Sectional Head on non-conformance";
                    }
                    else
                    {
                        _context.HeadDetps.Add(new HeadDetp
                        {
                            ObservationId = request.ObservationId,
                            FlowId = request.FlowId,
                            Decision = request.Decision,
                            Job = request.Job,
                            Remarks = request.Remarks,
                            DecisionById = userId,
                            DecisionDate = DateTime.Now
                        });
                        
                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = request.ObservationId,
                            TableName = "business_head",
                            FromId = userId,
                            ToId = observation.BusinessHeadId,
                            Date = DateTime.Now,
                            Completed = "N"
                        });
                        
                        observation.Status = "With Business Head";
                        
                        
                        
                        mailto = observation.BusinessHeadId.GetValueOrDefault();
                        subject = "Input given by QA Sectional Head on non-conformance";
                    }

                    currentFlow.Completed = "Y";
                    
                }
                else
                {


                    if (request.Decision == "Ok and Closed")
                    {
                        _context.HeadDetps.Add(new HeadDetp
                        {
                            ObservationId = request.ObservationId,
                            FlowId = request.FlowId,
                            Decision = request.Decision,
                            Remarks = request.Remarks,
                            DecisionById = userId,
                            DecisionDate = DateTime.Now
                        });
                        
                        observation.Status = "Closed by Head DETP";
                        
                        status = "Closed by Head DETP";
                        subject = "Non -conformance closed by head DETP.";
                    }

                    if (request.Decision == "Not Satisfied" || request.Decision == "Need Clarification")
                    {
                        _context.HeadDetps.Add(new HeadDetp
                        {
                            ObservationId = request.ObservationId,
                            FlowId = request.FlowId,
                            Decision = request.Decision,
                            Remarks = request.Remarks,
                            DecisionById = userId,
                            DecisionDate = DateTime.Now
                        });
                        
                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = request.ObservationId,
                            TableName = "business_head",
                            FromId = userId,
                            ToId = observation.BusinessHeadId,
                            Date = DateTime.Now,
                            Completed = "N"
                        });
                        
                        observation.Status = "With Business Head";
                        
                        status = "With Business Head";
                        
                        mailto = observation.BusinessHeadId.GetValueOrDefault();
                        subject = "Non-conformance returned by QA Sectional Head.";
                    }

                    if (request.Decision == "Forward to QA")
                    {
                        _context.HeadDetps.Add(new HeadDetp
                        {
                            ObservationId = request.ObservationId,
                            FlowId = request.FlowId,
                            Decision = request.Decision,
                            Remarks = request.Remarks,
                            DecisionById = userId,
                            DecisionDate = DateTime.Now
                        });
                        
                        var user = _context.UserRoles.Where(x => x.RoleId == RoleConst.HOD_QA_SHA).FirstOrDefault();
                        _context.QAFlows.Add(new QAFlow
                        {
                            ObservationId = request.ObservationId,
                            TableName = "hod_qa_sha",
                            FromId = userId,
                            ToId = user.UserId,
                            Date = DateTime.Now,
                            Completed = "N"
                        });
                        
                        
                        
                        observation.Status = "With HoD QA & SHA";
                        
                        status = "With HoD QA & SHA";
                        currentFlow.Completed = "Y";
                        

                        
                        mailto = user.UserId.GetValueOrDefault();
                        subject = "Non-conformance returned by HoD QA & SHA.";
                    }
                    currentFlow.Completed = "Y";
                    
                }
            }
            else
            {
                if (request.Decision == "Need Clarification")
                {
                    _context.HeadDetps.Add(new HeadDetp
                    {
                        ObservationId = request.ObservationId,
                        FlowId = request.FlowId,
                        Decision = request.Decision,
                        Remarks = request.Remarks,
                        DecisionById = userId,
                        DecisionDate = DateTime.Now
                    });

                    
                    
                    
                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = "dept_hod",
                        FromId = userId,
                        ToId = observation.DeptHodId,
                        Date = DateTime.Now,
                        Completed = "N"
                    });
                    
                    observation.Status = "With Dept HOD";
                    
                    
                    currentFlow.Completed = "Y";
                    

                    
                    mailto = observation.DeptHodId.GetValueOrDefault();

                }
                else
                {
                    
                    
                    var hodQaSha = _context.UserRoles.Where(x => x.RoleId == RoleConst.HOD_QA_SHA).FirstOrDefault();
                    
                    _context.QAFlows.Add(new QAFlow
                    {
                        ObservationId = request.ObservationId,
                        TableName = "hod_qa_sha",
                        FromId = userId,
                        ToId = hodQaSha.UserId,
                        Date = DateTime.Now,
                        Completed = "N"
                    });
                    observation.Status = "With HoD QA & SHA";
                    
                    
                    
                    
                    mailto = observation.DeptHodId.GetValueOrDefault();
                }
               
            }
            currentFlow.Completed = "Y";
            this.SaveFlowAtt(request.Att, request.FlowId);
            _context.SaveChanges();
            this.SendObservationUpdatedMail(request.ObservationId, subject, mailto);
            return observation.Status;
        }
    }
}
