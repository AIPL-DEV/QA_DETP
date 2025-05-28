using DETP.auth;
using DETP.data;
using DETP.extensions;
using DETP.model;
using DETP.requests.editObservation;
using DETP.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DETP.Controllers
{

    public class ChangeToModel
    {
        public string UserId { get; set; }
        public string FlowId { get; set; }
    }
    [Authorize]
    public class QAFlowController : Controller
    {
        readonly sqlhelp _ = new();
        private readonly ApplicationDbContext _context;
        private readonly ObservationFlowService _flowService;
        public QAFlowController(ApplicationDbContext applicationDbContext, ObservationFlowService flowService)
        {
            _context = applicationDbContext;
            _flowService = flowService;
        }

        // GET: QAFlowController
        public ActionResult Index(int page)
        {

            if (page == 0)
            {
                page = 1;
            }
            //if (HttpContext.Session.GetString("app_name").Equals("QA") && HttpContext.Session.GetString("role_name").Equals("Super Admin"))
            //{
            if (HttpContext.Session.Get("user") != null)
            {
                ViewBag.Title = "QA Flow";
                DataTable my_observation = null, observations_for_me = null;

                var query = @$"SELECT * FROM (select serial_no,
                            visit_no, 
                            status, 
                            ROW_NUMBER() OVER (ORDER BY serial_no DESC) as row
                            from qa_observation q
                            ) a";

                sqlhelp.fetch1(query);

                if (sqlhelp.datatable1.Rows.Count > 0)
                {
                    my_observation = sqlhelp.datatable1;
                }

                DataTable values = my_observation;

                sqlhelp.fetch1($"SELECT COUNT(*) FROM qa_observation");
                int total = 0;
                if (sqlhelp.datatable1.Rows.Count > 0)
                {
                    total = int.Parse(sqlhelp.datatable1.Rows[0].ItemArray[0].ToString());
                }

                ViewBag.from = page * 10 - 10;
                ViewBag.to = page * 10;
                ViewBag.total = total;
                ViewBag.totalPage = total / 10 + 2;
                ViewBag.page = page;
                ViewBag.prev = page - 1;
                if (total / 10 + 2 > page + 1)
                {
                    ViewBag.next = page + 1;
                }

                return View(values);
            }
            else
                return RedirectToAction("Login", "Account");
            // }
            //else
            //return RedirectToAction("Login", "Account");

        }

        // GET: QAFlowController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }



        // GET: QAFlowController/Edit/5
        public ActionResult Edit(int id)
        {

            sqlhelp.fetch1($@"SELECT qa_flow.*, t.name as to_name ,f.name as from_name FROM qa_flow 
                            LEFT JOIN users as t ON t.user_id = qa_flow.to_id
                            LEFT JOIN users as f ON f.user_id = qa_flow.from_id
                            
                            WHERE observation_id={id}  ORDER BY id ASC");

            var data = sqlhelp.datatable1;
            List<List<String>> imageList = new List<List<string>>();
            data.Columns.Add("imgs");
            for (int i = 0; i < data.Rows.Count; i++)
            {
                sqlhelp.fetch1($"SELECT data FROM qa_att WHERE type_id={data.Rows[i].ItemArray[0]} AND type='flow'");
                var imgList = new List<string>();
                if (sqlhelp.datatable1.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in sqlhelp.datatable1.Rows)
                    {
                        imgList.Add(dataRow.ItemArray[0].ToString());
                    }
                }
                imageList.Add(imgList);

            }
            ViewBag.imageList = imageList;

            sqlhelp.fetch1($@"SELECT user_id,name,email,pno FROM users");


            ViewBag.users = sqlhelp.datatable1;
            sqlhelp.fetch1($@"SELECT * FROM qa_observation where serial_no={id}");
            ViewBag.observation = _context.QAObservations
                .Include(x => x.SiteIncharge)
                .Include(x => x.ProjectIncharge)
                .Include(x => x.DeptHod)
                .Include(x => x.HeadDetp)
                .Include(x => x.BusinessHead)
                .Where(x => x.SerialNo == id).FirstOrDefault();
            sqlhelp.fetch1($"SELECT data FROM qa_att WHERE type_id={sqlhelp.datatable1.Rows[0].ItemArray[0]} AND type='observation'");
            var oImgList = new List<string>();

            if (sqlhelp.datatable1.Rows.Count > 0)
            {
                foreach (DataRow dataRow in sqlhelp.datatable1.Rows)
                {
                    oImgList.Add(dataRow.ItemArray[0].ToString());
                }
            }
            ViewBag.oImageList = oImgList;

            ViewBag.flowCount = _context.QAFlows.Where(x => x.ObservationId == id).Count();


            return View(data);
        }




        [HttpPost]
        public ActionResult ChangeTo([FromBody] ChangeToModel changeTo)
        {
            sqlhelp.performAction($"UPDATE qa_flow SET to_id={changeTo.UserId} WHERE id={changeTo.FlowId}");
            return Json(new { status = "success" });
        }

        // GET: QAFlowController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: QAFlowController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult AddImage(string type, string type_id, string imgs)
        {

            var files = imgs.ToList();
            var base64List = new List<string>();
            var jArray = JArray.Parse(imgs);
            var arr = jArray.Children();
            foreach (var item in arr)
            {

                sqlhelp.performAction($"INSERT INTO qa_att (type,type_id,data)VALUES('{type}','{type_id}','{item}')");
            }

            return Json(new { status = true });
        }
        public ActionResult DeleteImage(string type, string type_id, string image, IFormFileCollection imgs)
        {


            sqlhelp.fetch1($"SELECT data FROM qa_att WHERE type='{type}' AND type_id={type_id} AND data LIKE '%{image}%'");
            var data = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
            if (data == image)
            {
                sqlhelp.performAction($"DELETE FROM qa_att WHERE type='{type}' AND type_id={type_id} AND data LIKE '%{image}%'");
            }
            else
            {
                data = data.Replace(image, "");
                sqlhelp.performAction($"UPDATE qa_att SET data={data} WHERE type='{type}' AND type_id={type_id} AND data LIKE '%{image}%'");
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public ActionResult AddNewFlow(string changeComplainceDate, int userId, DateTime complianceTargetDate, int observationId)
        {
            var observation = _context.QAObservations.Where(x => x.SerialNo == observationId).FirstOrDefault();
            var fromId = observation.ObservationById;
            var toId = userId;
            if (changeComplainceDate == "Yes")
            {
                observation.ComplianceTargetDate = complianceTargetDate;
            }

            var tableName = "";
            switch (observation.Status)
            {
                case "With Site Incharge":
                    tableName = "site_incharge";
                    break;
                case "With Project Incharge":
                    tableName = "project_incharge";
                    break;
                case "With Dept HOD":
                    tableName = "dept_hod";
                    break;
                case "With Business Head":
                    tableName = "business_head";
                    break;
                case "With Head DETP":
                    tableName = "head_detp";
                    break;
                case "With Assignee Section":
                    tableName = "assignee";
                    break;
                case "With QA Officer":
                    tableName = "qa_officer";
                    break;
                default:
                    break;
            }

            _context.QAFlows.Add(new model.QAFlow
            {
                ObservationId = observationId,
                Completed = "N",
                FromId = fromId,
                ToId = toId,
                Date = observation.ObservationDate,
                TableName = tableName,
            });

            _context.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ActionName("SaveSiteIncharge")]
        public IActionResult SaveSiteIncharge(SiteInchargeCreateRequest request)
        {
            var result = request.Validate();
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            var status = _flowService.SaveSiteIncharge(request, this.AuthUserId());

            return Ok(new
            {
                Status = status
            });
        }

        [HttpPost]
        [ActionName("SaveProjectIncharge")]
        public IActionResult SaveProjectIncharge(ProjectInchargeCreateRequest request)
        {
            var result = request.Validate();
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            var status = _flowService.SaveProjectIncharge(request, this.AuthUserId());

            return Ok(new
            {
                Status = status
            });
        }

        [HttpPost]
        [ActionName("SaveQaOfficer")]
        public IActionResult SaveQaOfficer(QAOfficerCreateRequest request)
        {   
            var result = request.Validate();
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            var status = _flowService.SaveQAOfficer(request, this.AuthUserId());

            return Ok(new { Status = status });
        }

        [HttpPost]
        [ActionName("SaveBusinessHead")]
        public IActionResult SaveBusinessHead(BusinessHeadCreateRequest request)
        {
            var result = request.Validate();
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            var status = _flowService.SaveBusinessHead(request, this.AuthUserId());

            return Ok(new { Status = status });
        }

        [HttpPost]
        [ActionName("SaveAssignee")]
        public IActionResult SaveAssignee(AssigneeCreateRequest request)
        {
            var result = request.Validate();
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            var status = _flowService.SaveAssignee(request, this.AuthUserId());

            return Ok(new { Status = status });
        }

        [HttpPost]
        [ActionName("SaveDeptHod")]
        public IActionResult SaveDeptHod(DeptHodCreateRequest request)
        {
            var result = request.Validate();
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            var status = _flowService.SaveDeptHod(request, this.AuthUserId());

            return Ok(new { Status = status });
        }

        [HttpPost]
        [ActionName("SaveHeadDetp")]
        public IActionResult SaveHeadDetp(HeadDetpCreateRequest request)
        {
            var result = request.Validate();
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            var status = _flowService.SaveHeadDetp(request, this.AuthUserId());

            return Ok(new { Status = status });
        }

        [HttpPost]
        [ActionName("SaveHodQaSha")]
        public IActionResult SaveHodQaSha(HodQaShaCreateRequest request)
        {
            var result = request.Validate();
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            var status = _flowService.SaveHodQaSha(request, this.AuthUserId());

            return Ok(new { Status = status });
        }

    }
}
