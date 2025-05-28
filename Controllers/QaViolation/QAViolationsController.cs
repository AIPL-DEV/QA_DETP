using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Razor;
using DETP.auth;
using DETP.Constant;
using DETP.data;
using DETP.Email;
using DETP.extensions;
using DETP.model;
using DETP.model.QaViolation;
using DETP.requests.QaViolation;
using DETP.services;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml.Table.PivotTable;
using Razor.Templating.Core;

namespace DETP.Controllers.QaViolation
{
    [Authorize]
    public class QAViolationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration Configuration;
        public QAViolationsController(ApplicationDbContext context, IConfiguration configuration)
        {
            this._context = context;
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "QA Violation";
            return View();
        }

        public IActionResult Search([FromForm] int Year = 0, int Month = 0, string VendorCode = null)
        {
            int totalRecord = 0;
            int filterRecord = 0;

            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "10");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var searchFieldName = Request.Form["fieldName"].FirstOrDefault();
            var searchFieldValue = Request.Form["fieldValue"].FirstOrDefault();
            var deptIds = Request.Form["departmentIds"].FirstOrDefault();
            var divsIds = Request.Form["divisionIds"].FirstOrDefault();

            List<string> department = new();
            List<long> division = new();
            if (deptIds != null)
            {
                department = JsonConvert.DeserializeObject<List<string>>(deptIds);
            }

            if (divsIds != null)
            {
                division = JsonConvert.DeserializeObject<List<long>>(divsIds);
            }


            IQueryable<model.QAObservation> data = _context.QAObservations
                .Include(x => x.Division)
                .Include(x => x.QaFlows)
                .ThenInclude(x => x.To);

            if (department?.Count > 0)
            {
                data = data.Where(x => department.Contains(x.DepartmentId));
            }

            if (division?.Count > 0)
            {
                data = data.Where(x => division.Contains(x.DivisionId.Value));
            }

            totalRecord = data.Count();

            if (Year != 0)
            {
                data = data.Where(x => x.LoggedDate.HasValue && x.LoggedDate.Value >= new DateTime(Year - 1, 4, 1) && x.LoggedDate.Value <= new DateTime(Year, 3, 31));
            }

            if (Month != 0)
            {
                data = data.Where(x => x.LoggedDate.HasValue && x.LoggedDate.Value.Month == Month);
            }

            if (!string.IsNullOrEmpty(VendorCode))
            {
                data = data.Where(x => x.VendorCode == VendorCode);
            }


            // search data when search value found
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.Trim().ToLower();
                data = data.Where(x =>
                    x.Division.name.Contains(searchValue) ||
                    x.SerialNo.ToString().Contains(searchValue) ||
                    x.VisitNo.Contains(searchValue) ||
                    x.Status.Contains(searchValue) ||
                    x.LoggedDate.ToString().Contains(searchValue) ||
                    x.Site.Contains(searchValue) ||
                    x.Location.Contains(searchValue) ||
                    x.NatureOfWork.Contains(searchValue) ||
                    x.TypeOfObservation.Contains(searchValue) ||
                    x.TypeOfConfirmance.Contains(searchValue) ||
                    x.NatureOfConfirmance.Contains(searchValue) ||
                    x.Standard.Contains(searchValue) ||
                    x.Basics.Contains(searchValue) ||
                    x.VendorCode.Contains(searchValue) ||
                    x.VendorName.Contains(searchValue)
                );

            }
            // get total count of records after search
            filterRecord = data.Count();

            if (!string.IsNullOrEmpty(sortColumnDirection) && !string.IsNullOrEmpty(sortColumn))
            {
                if (sortColumnDirection == "asc")
                {
                    data = sortColumn switch
                    {
                        "Division" => data.OrderBy(x => x.Division.name),
                        "SerialNo" => data.OrderBy(x => x.SerialNo),
                        "VisitNo" => data.OrderBy(x => x.VisitNo),
                        "Status" => data.OrderBy(x => x.Status),
                        "LoggedDate" => data.OrderBy(x => x.LoggedDate),
                        "Site" => data.OrderBy(x => x.Site),
                        "Location" => data.OrderBy(x => x.Location),
                        "NatureOfWork" => data.OrderBy(x => x.NatureOfWork),
                        "TypeOfObservation" => data.OrderBy(x => x.TypeOfObservation),
                        "TypeOfConfirmance" => data.OrderBy(x => x.TypeOfConfirmance),
                        "NatureOfConfirmance" => data.OrderBy(x => x.NatureOfConfirmance),
                        "Standard" => data.OrderBy(x => x.Standard),
                        "Basics" => data.OrderBy(x => x.Basics),
                        "VendorCode" => data.OrderBy(x => x.VendorCode),
                        "VendorName" => data.OrderBy(x => x.VendorName),

                        _ => data.OrderBy(x => x.SerialNo),
                    };
                }
                if (sortColumnDirection == "desc")
                {
                    data = sortColumn switch
                    {
                        "Division" => data.OrderByDescending(x => x.Division.name),
                        "SerialNo" => data.OrderByDescending(x => x.SerialNo),
                        "VisitNo" => data.OrderByDescending(x => x.VisitNo),
                        "Status" => data.OrderByDescending(x => x.Status),
                        "LoggedDate" => data.OrderByDescending(x => x.LoggedDate),
                        "Site" => data.OrderByDescending(x => x.Site),
                        "Location" => data.OrderByDescending(x => x.Location),
                        "NatureOfWork" => data.OrderByDescending(x => x.NatureOfWork),
                        "TypeOfObservation" => data.OrderByDescending(x => x.TypeOfObservation),
                        "TypeOfConfirmance" => data.OrderByDescending(x => x.TypeOfConfirmance),
                        "NatureOfConfirmance" => data.OrderByDescending(x => x.NatureOfConfirmance),
                        "Standard" => data.OrderByDescending(x => x.Standard),
                        "Basics" => data.OrderByDescending(x => x.Basics),
                        "VendorCode" => data.OrderByDescending(x => x.VendorCode),
                        "VendorName" => data.OrderByDescending(x => x.VendorName),

                        _ => data.OrderByDescending(x => x.SerialNo),
                    };
                }
            }

            var resultList = data.Skip(skip).Take(pageSize).ToList();
            var departmentIds = resultList.Select(x => x.DepartmentId).ToList();
            var departments = _context.Department.Where(x => departmentIds.Contains(x.Id.ToString())).ToList();

            foreach (var result in resultList)
            {
                result.Dept = departments.Where(x => x.Id.ToString() == result.DepartmentId).FirstOrDefault();
            }

            var dataForSend = resultList.Select(x =>
            {
                var qaFlow = x.QaFlows.Where(x => x.Completed == "N").LastOrDefault();

                if (qaFlow != null)
                {
                    var name = qaFlow.To.Name.Trim();
                    var pno = qaFlow.To.PNo.Trim();
                    x.Status += $"<br /><span class='badge badge-primary'>({name} - {pno})<span>";
                }

                x.QaFlows = null;

                return x;
            });

            var returnObj = Json(new
            {
                draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = dataForSend
            });

            return returnObj;
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            ViewBag.Title = "Vendor Violation";
            ViewBag.UserName = _context.Users.Where(x => x.UserId == this.AuthUserId()).FirstOrDefault().Name;
            var observation = _context.QAObservations.Include(x=>x.Division).FirstOrDefault(x => x.SerialNo == id);
            ViewBag.Observation = observation;
            ViewBag.Categories = new SelectList(_context.QaViolationCategories, "Id", "Name");
            ViewBag.SubCategories = new SelectList(_context.QaViolationSubCategories, "Id", "Name");
            ViewBag.ViolationCount = 1 + _context.QaViolations.Where(x => x.Observation.VendorCode == observation.VendorCode && x.QAStatus != (int)QaViolationStatus.REJECTED).Count();
            
            
            observation.Dept = _context.Department.Where(x => x.Id.ToString() == observation.DepartmentId).FirstOrDefault();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(QaViolationCreateRequest request)
        {
            using var transaction = _context.Database.BeginTransaction();
            var observation = _context.QAObservations.FirstOrDefault(x => x.SerialNo == request.ObservationId);
            var QaViolation = new model.QaViolation.QaViolation
            {
                VendorEmail = request.VendorEmail,
                ObservationDetails = request.ObservationDetails,
                Amount = request.Amount,
                ObservationId = request.ObservationId,
                QaViolationCategoryId = request.ViolationCategory,
                QaViolationSubCategoryId = request .SubCategory,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedById = this.AuthUserId(),
                Status = QaViolationStatus.WITH_HEAD_QA_SHA,
                Count = 1 + _context.QaViolations.Where(x => x.Observation.VendorCode == observation.VendorCode).Count()
            };

            


            if (request.Attachment != null)
            {
                var fileName = request.Attachment.FileName;
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ViolationAttachments");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    request.Attachment.CopyTo(stream);
                }
                QaViolation.Attachment = fileName;
            }

            _context.QaViolations.Add(QaViolation);
            _context.SaveChanges();
            var role = _context.Roles.FirstOrDefault(x => x.Name == RoleNameConst.HOD_QA_SHA);

            var toUser = _context.UserRoles.FirstOrDefault(x => x.RoleId == role.Id);

            if(toUser == null)
            {
                transaction.Rollback();
                return Problem(statusCode: 500, title: "Hod QA & SHA Not found");
            }

            var tableName = _context.Model.FindEntityType(typeof(QaViolationApproval))?.GetTableName();



            var flow = new QaViolationFlow
            {
                QaViolationId = QaViolation.Id,
                FromId = this.AuthUserId(),
                ToId = toUser.UserId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                TableName = _context.GetTableName<QaViolationApproval>(),
                Completed = false,
                ToRoleId = role.Id
            };

            _context.QaViolationFlows.Add(flow);

            _context.SaveChanges();
            transaction.Commit();

            var html = await ViewRenderService.RenderViewToStringAsync("RequesterEmail", QaViolation.Id, this.ControllerContext);

            var user = _context.Users.Where(x => x.UserId == toUser.UserId).FirstOrDefault();
            var requester = _context.Users.Where(x => x.UserId == QaViolation.CreatedById).FirstOrDefault();
            var extraCC = (Configuration["QaViolationCC"] ?? "").Split(",");
            var ccs = _context.Users.Where(x => extraCC.Contains(x.PNo)).Select(x=>x.Email).ToList();
            ccs.Add(requester.Email);

            mail.SendMail(new DetpMailMessage
            {
                To = user.Email,
                Subject = "Consequnce-Quality Non-Conformance",
                Body = html,
                CC = ccs.ToArray(),
                Attachments = new string[] { QaViolation.AttachmentFullPath }
            });

            return Ok(QaViolation);
        }

        public IActionResult Pending(string status)
        {
            ViewBag.Title = "Pending Violation";

            var userRole = _context.UserRoles.Where(x => x.UserId == this.AuthUserId()).Select(x=>x.RoleId).ToList();
            var roles = _context.Roles.Where(x => userRole.Contains(x.Id)).Select(x=>x.Name).ToList();

            
            var violations = _context.QaViolationFlows.Where(x => x.ToId == this.AuthUserId() && !x.Completed).Select(x => x.QaViolationId).ToList();
            IQueryable<model.QaViolation.QaViolation> observationsQuery = _context.QaViolations
                .Include(x => x.Observation);
            if(status == "OPEN")
            {
                observationsQuery = observationsQuery.Where(x => x.QAStatus != (int)QaViolationStatus.REJECTED && (int)QaViolationStatus.FINISHED != x.QAStatus);
            }
            else if (status == "CLOSED")
            {
                observationsQuery = observationsQuery.Where(x => x.QAStatus == (int)QaViolationStatus.REJECTED && (int)QaViolationStatus.FINISHED == x.QAStatus);
            }
            // else if(status == QaViolationStatus.WITH_HEAD_QA_SHA)
            //   {

            //   }
            else if (status == QaViolationStatus.WITH_HEAD_QA_SHA.ToString())
            {
            }


            var groupObservations = observationsQuery.OrderByDescending(x => x.Id).ToList().GroupBy(x => x.Observation.VendorCode).ToList();
            return View(groupObservations);
        }
        
        public IActionResult Action(long id)
        {
            var violation = _context.QaViolations.Include(x=>x.SubCategory.PenaltyDetail).Include(x=>x.Category).Include(x=>x.CreatedBy).Include(x=>x.Observation.Division).Where(x => x.Id == id).FirstOrDefault();
            violation.Observation.Dept = _context.Department.Where(x => x.Id.ToString() == violation.Observation.DepartmentId).FirstOrDefault();
            var flows = _context.QaViolationFlows.Include(x=>x.ToRole).Where(x => x.QaViolationId == violation.Id).OrderBy(x => x.CreatedAt).ToList();
            List<dynamic> flowDecisions = new List<dynamic>();
            foreach (var item in flows)
            {
                if (item.Completed) {
                    if (item.TableName == _context.GetTableName<QaViolationApproval>())
                    {
                        flowDecisions.Add(_context.QaViolationApproval.Include(x=>x.ApprovedBy).Where(x => x.QaViolationFlowId == item.Id).FirstOrDefault());
                    }
                    if(item.TableName == _context.GetTableName<QaViolationCFOReview>())
                    {
                        flowDecisions.Add(_context.QaViolationCFOReviews.Include(x=>x.ApprovedBy).Where(x => x.QaViolationFlowId == item.Id).FirstOrDefault());
                    }
                    if(item.TableName == _context.GetTableName<QaViolationHeadProcurement>())
                    {
                        flowDecisions.Add(_context.QaViolationHeadProcurements.Include(x=>x.ApprovedBy).Where(x => x.QaViolationFlowId == item.Id).FirstOrDefault());
                    }
                }
            }

            ViewBag.Violation = violation;
            ViewBag.CurrentFlow = flows.Where(x=>!x.Completed && x.ToId == this.AuthUserId()).FirstOrDefault();
            ViewBag.FlowDecisions = flowDecisions;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Approval(ApprovalRequest request)
        {
            _context.Database.BeginTransaction();
            var currentFlow = _context.QaViolationFlows.Include(x=>x.ToRole).Where(x => x.Id == request.FlowId).FirstOrDefault();
            currentFlow.Completed = true;
            currentFlow.UpdatedAt = DateTime.Now;

            var approval = new QaViolationApproval
            {
                ApprovedById = this.AuthUserId(),
                Comments = request.Comment,
                CreatedAt = DateTime.Now,
                PenaltyAmountCorrect = request.PenaltyAmountCorrect,
                PenaltyClauseCorrect = request.PenaltyClauseCorrect,
                QaViolationFlowId = request.FlowId,
                QaViolationId = currentFlow.QaViolationId,
                RoleId = currentFlow.ToRoleId ?? 0,
                UpdatedAt = DateTime.Now,
            };
            if (request.Attachment != null)
            {
                var fileName = request.Attachment.FileName;
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ViolationAttachments");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    request.Attachment.CopyTo(stream);
                }
                approval.Attachment = fileName;
            }
            _context.QaViolationApproval.Add(approval);
            _context.SaveChanges();
            UserRole? nextUser = null;

            if(currentFlow.ToRole.Name == RoleNameConst.HOD_QA_SHA)
            {
                var nextRoleId = _context.Roles.Where(x => x.Name == RoleNameConst.GM_TECHNICAL_SERVICES).FirstOrDefault()?.Id;
                nextUser = _context.UserRoles.Where(x => x.RoleId == nextRoleId).FirstOrDefault();
            }
            if(currentFlow.ToRole.Name == RoleNameConst.GM_TECHNICAL_SERVICES)
            {
                var nextRoleId = _context.Roles.Where(x => x.Name == RoleNameConst.CFO).FirstOrDefault()?.Id;
                nextUser = _context.UserRoles.Where(x => x.RoleId == nextRoleId).FirstOrDefault();
            }

            if(nextUser == null)
            {
                return NotFound("user not found");
            }

            var tableName = _context.GetTableName<QaViolationApproval>();
            if(currentFlow.ToRole.Name == RoleNameConst.GM_TECHNICAL_SERVICES)
            {
                tableName = _context.GetTableName<QaViolationCFOReview>();
            }

            var qaViolation = _context.QaViolations.Where(x => currentFlow.QaViolationId == x.Id).FirstOrDefault();

            if (request.Decision == "Reject")
            {
                qaViolation.Status = QaViolationStatus.REJECTED;
                _context.SaveChanges();
                _context.Database.CommitTransaction();

                var body = await ViewRenderService.RenderViewToStringAsync("ApprovalRejectedEmail", qaViolation.Id, this.ControllerContext);
                var requester1 = _context.Users.Where(x => x.UserId == qaViolation.CreatedById).FirstOrDefault();
                var requestiorPno1 = (Configuration["QaViolationCC"] ?? "").Split(",");
                var ccs1 = _context.Users.Where(x => requestiorPno1.Contains(x.PNo)).Select(x => x.Email).ToList();
                mail.SendMail(requester1.Email, "Consequnce-Quality Non-Conformance", body, requester1.Email + "," + string.Join(",", ccs1));

                return Ok(new
                {
                    message = "Successfully submitted!"
                });
            }

            if (currentFlow.ToRole.Name == RoleNameConst.HOD_QA_SHA)
            {
                qaViolation.Status = QaViolationStatus.WITH_GM_TECHNICAL_SERVICES;
            }
            if (currentFlow.ToRole.Name == RoleNameConst.GM_TECHNICAL_SERVICES)
            {
                qaViolation.Status = QaViolationStatus.WITH_CFO;
            }

            _context.QaViolationFlows.Add(new QaViolationFlow
            {
                FromId = this.AuthUserId(),
                ToId = nextUser.UserId,
                Completed = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                QaViolationId = currentFlow.QaViolationId,
                TableName = tableName,
                ToRoleId = nextUser.RoleId
            });
            var viewName = "ApprovalEmail";
            
            
            var user = _context.Users.Where(x => x.UserId == nextUser.UserId).FirstOrDefault();
            var requester = _context.Users.Where(x => x.UserId == qaViolation.CreatedById).FirstOrDefault();
            var requestiorPno = (Configuration["QaViolationCC"] ?? "").Split(",");
            var ccs = _context.Users.Where(x => requestiorPno.Contains(x.PNo)).Select(x => x.Email).ToList();
            ccs.Add(requester.Email);
            if (qaViolation.Status == QaViolationStatus.WITH_CFO)
            {
                viewName = "CfoApprovalEmail";
                var headQa = _context.Users.Where(x=>x.UserId == currentFlow.FromId).FirstOrDefault();
                ccs.Add(headQa.Email);
            }

            var html = await ViewRenderService.RenderViewToStringAsync(viewName, qaViolation.Id, this.ControllerContext);
            mail.SendMail(new DetpMailMessage
            {
                To = user.Email,
                Subject = "Consequnce-Quality Non-Conformance",
                Body = html,
                CC = ccs.ToArray(),
                Attachments = new string[] { qaViolation.AttachmentFullPath }
            });
            _context.SaveChanges();
            _context.Database.CommitTransaction();
            return Ok(new
            {
                message = "Successfully submitted!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> CFOSubmit(CFOSubmitRequest request)
        {
            _context.Database.BeginTransaction();
            var currentFlow = _context.QaViolationFlows.Include(x => x.ToRole).Where(x => x.Id == request.FlowId).FirstOrDefault();
            currentFlow.Completed = true;
            currentFlow.UpdatedAt = DateTime.Now;

            var approval = new QaViolationCFOReview
            {
                ApprovedById = this.AuthUserId(),
                Comments = request.Comment,
                CreatedAt = DateTime.Now,
                DeducationDate = request.DeducationDate,
                DebitNote = request.DebitNote,
                QaViolationFlowId = request.FlowId,
                QaViolationId = currentFlow.QaViolationId,
                UpdatedAt = DateTime.Now,
            };
            
            _context.QaViolationCFOReviews.Add(approval);
            _context.SaveChanges();


            var nextRoleId = _context.Roles.Where(x => x.Name == RoleNameConst.HEAD_PROCUREMENT).FirstOrDefault()?.Id;
            UserRole nextUser = _context.UserRoles.Where(x => x.RoleId == nextRoleId).FirstOrDefault();
           

            if (nextUser == null)
            {
                return NotFound("user not found");
            }
            var tableName = _context.GetTableName<QaViolationHeadProcurement>();

            var qaViolation = _context.QaViolations.Include(x=>x.Observation).Where(x => currentFlow.QaViolationId == x.Id).FirstOrDefault();


            qaViolation.Status = QaViolationStatus.WITH_HEAD_PROCUREMENT;
            

            _context.QaViolationFlows.Add(new QaViolationFlow
            {
                FromId = this.AuthUserId(),
                ToId = nextUser.UserId,
                Completed = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                QaViolationId = currentFlow.QaViolationId,
                TableName = tableName,
                ToRoleId = nextUser.RoleId
            });
            _context.SaveChanges();
            _context.Database.CommitTransaction();

            var role = _context.Roles.FirstOrDefault(x => x.Name == RoleNameConst.HOD_QA_SHA);

            var toUser = _context.UserRoles.FirstOrDefault(x => x.RoleId == role.Id);
            var user = _context.Users.Where(x => x.UserId == nextUser.UserId).FirstOrDefault();
            var requester = _context.Users.Where(x => x.UserId == qaViolation.CreatedById).FirstOrDefault();
            var requestiorPno = (Configuration["QaViolationCC"] ?? "").Split(",");
            var ccs = _context.Users.Where(x => requestiorPno.Contains(x.PNo)).Select(x => x.Email).ToList();
            ccs.Add(requester.Email);

            var html = await ViewRenderService.RenderViewToStringAsync("HeadProcurementEmail", qaViolation, this.ControllerContext);
            mail.SendMail(new DetpMailMessage
            {
                To = user.Email,
                Subject = "Consequnce-Quality Non-Conformance",
                Body = html,
                CC = ccs.ToArray(),
                Attachments = new string[] { qaViolation.AttachmentFullPath }
            });
            
            return Ok(new
            {
                message = "Successfully submitted!"
            });
        }


        [HttpPost]
        public async Task<IActionResult> HeadProcurementSubmit(HeadProcurementSubmitRequest request)
        {
            _context.Database.BeginTransaction();
            var currentFlow = _context.QaViolationFlows.Include(x => x.ToRole).Where(x => x.Id == request.FlowId).FirstOrDefault();
            currentFlow.Completed = true;
            currentFlow.UpdatedAt = DateTime.Now;

            var approval = new QaViolationHeadProcurement
            {
                ApprovedById = this.AuthUserId(),
                Comments = request.Comment,
                CreatedAt = DateTime.Now,
                QaViolationFlowId = request.FlowId,
                QaViolationId = currentFlow.QaViolationId,
                UpdatedAt = DateTime.Now,
            };

            if (request.Attachment != null)
            {
                var fileName = request.Attachment.FileName;
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ViolationAttachments");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    request.Attachment.CopyTo(stream);
                }
                approval.Attachment = fileName;
            }

            _context.QaViolationHeadProcurements.Add(approval);
            _context.SaveChanges();


            var nextRoleId = _context.Roles.Where(x => x.Name == RoleNameConst.HEAD_PROCUREMENT).FirstOrDefault()?.Id;
            UserRole nextUser = _context.UserRoles.Where(x => x.RoleId == nextRoleId).FirstOrDefault();


            if (nextUser == null)
            {
                return NotFound("user not found");
            }

            var tableName = _context.GetTableName<QaViolationApproval>();
            if (currentFlow.ToRole.Name == RoleNameConst.GM_TECHNICAL_SERVICES)
            {
                tableName = _context.GetTableName<QaViolationCFOReview>();
            }

            var qaViolation = _context.QaViolations.Where(x => currentFlow.QaViolationId == x.Id).FirstOrDefault();

            qaViolation.Status = QaViolationStatus.FINISHED;

            var requester = _context.Users.Where(x => x.UserId == qaViolation.CreatedById).FirstOrDefault();
            var requestiorPno = (Configuration["QaViolationCC"] ?? "").Split(",");
            var ccs = _context.Users.Where(x => requestiorPno.Contains(x.PNo)).Select(x => x.Email).ToList();
            ccs.Add(requester.Email);

            var html = await ViewRenderService.RenderViewToStringAsync("VendorEmail", qaViolation.Id, this.ControllerContext);

            mail.SendMail(qaViolation.VendorEmail, "Consequnce-Quality Non-Conformance", html, requester.Email + "," + string.Join(",", ccs), new string[] { approval.AttachmentFullPath });
            mail.SendMail(new DetpMailMessage
            {
                To = qaViolation.VendorEmail,
                Subject = "Consequnce-Quality Non-Conformance",
                Body = html,
                BCC = ccs.ToArray()
            });

            _context.SaveChanges();
            _context.Database.CommitTransaction();
            return Ok(new
            {
                message = "Successfully submitted!"
            });
        }

        public class QaViolationDashboardModel
        {
            public int TotalViolation { get; set; }
            public int Open { get; set; }
            public int Close { get; set; }
            public int QaOfficer { get; set; }
            public int QaHead { get; set; }
            public int GmTechnicalServices { get; set; }
            public int CFO { get; set; }
            public int HeadProcurement { get; set; }
        }

        public IActionResult Dashboard()
        {
            QaViolationDashboardModel model = new QaViolationDashboardModel
            {
                TotalViolation = _context.QaViolations.Count(),
                Open = _context.QaViolations.Where(x=>x.QAStatus != (int)QaViolationStatus.REJECTED && x.QAStatus != (int)QaViolationStatus.FINISHED).Count(),
                CFO = _context.QaViolations.Where(x=>x.QAStatus == (int)QaViolationStatus.WITH_CFO).Count(),
                GmTechnicalServices = _context.QaViolations.Where(x => x.QAStatus == (int)QaViolationStatus.WITH_GM_TECHNICAL_SERVICES).Count(),
                HeadProcurement = _context.QaViolations.Where(x => x.QAStatus == (int)QaViolationStatus.WITH_HEAD_PROCUREMENT).Count(),
                QaHead = _context.QaViolations.Where(x => x.QAStatus == (int)QaViolationStatus.WITH_HEAD_QA_SHA).Count()
            };

            model.Close = model.TotalViolation - model.Open;

            _context.QaViolations.Count();
            return View(model);
        }

        public IActionResult ByVendorCode(string id)
        {
            var response = _context.QaViolations.Include(x => x.Observation).Where(x => x.Observation.VendorCode == id).ToList();
            return Json(response);
        }
    }
}
