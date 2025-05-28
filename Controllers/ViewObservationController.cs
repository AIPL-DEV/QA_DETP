using DETP.auth;
using DETP.data;
using DETP.model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DETP
{
    public class Field
    {
        public string Name { get; set; }
        public string Column { get; set; }
    }
    [Authorize(app: new string[] { "QA" })]
    public class ViewObservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ViewObservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string CreateStatus(string data)
        {
            data = data.Replace("Head DETP", "Sectional Head QA");
            data = data.Replace("With EIC DETP", "With HoD DETP");
            data = data.Replace("EIC DETP", "HoD DETP");
            data = data.Replace("EIC DETP", "HoD DETP");
            data = data.Replace("EIC DETP", "HoD DETP");
            data = data.Replace("EIC DETP", "HoD DETP");
            return data.Replace("Closed by EIC DETP", "Closed");
        }

        [ActionName("List")]
        public IActionResult List(bool observationByMe = false, int departmentId = 0)
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


            IQueryable<QAObservation> data = _context.QAObservations
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

            if (observationByMe)
            {
                data = data.Where(x => x.QaFlows.Where(x => x.ToId == HttpContext.Session.GetInt32("user") && x.Completed == "N").Any());
            }

            if (departmentId > 0)
            {
                data = data.Where(x => x.DepartmentId == departmentId.ToString());
            }

            if (!string.IsNullOrEmpty(searchFieldValue))
            {
                searchFieldValue = searchFieldValue.Trim().ToLower();
                searchFieldValue = CreateStatus(searchFieldValue);
                data = searchFieldName switch
                {
                    "serial_no" => data.Where(x => x.SerialNo.ToString().Contains(searchFieldValue)),
                    "visit_no" => data.Where(x => x.VisitNo.Contains(searchFieldValue)),
                    "status" => searchFieldValue.ToLower() == "with hod detp" ? data.Where(x => x.Status.Contains("With EIC DETP")).Where(x=>x.ObservationDate.Value.Date >= new DateTime(2022,12,03).Date) : data.Where(x => x.Status.Contains(searchFieldValue)),
                    "logged_date" => data.Where(x => x.LoggedDate.Value.Date == DateTime.Parse(searchFieldValue).Date),
                    "department" => data.Where(x => _context.Department.Where(y => y.Abbr.Contains(searchFieldValue)).Select(x => x.Id.ToString()).ToList().Contains(x.DepartmentId)),
                    "division" => data.Where(x => x.Division.name.StartsWith(searchFieldValue)),
                    "site" => data.Where(x => x.Site.Contains(searchFieldValue)),
                    "location" => data.Where(x => x.Location.Contains(searchFieldValue)),
                    "nature_of_work" => data.Where(x => x.NatureOfWork.Contains(searchFieldValue)),
                    "type_of_observation" => data.Where(x => x.TypeOfObservation.Contains(searchFieldValue)),
                    "type_of_confirmance" => data.Where(x => x.TypeOfConfirmance.Contains(searchFieldValue)),
                    "nature_of_confirmance" => data.Where(x => x.NatureOfConfirmance.Contains(searchFieldValue)),
                    "standard" => data.Where(x => x.Standard.Contains(searchFieldValue)),
                    "basics" => data.Where(x => x.Basics.Contains(searchFieldValue)),
                    "vendor_code" => data.Where(x => x.VendorCode.Contains(searchFieldValue)),
                    "vendor_name" => data.Where(x => x.VendorName.Contains(searchFieldValue)),
                    _ => data,
                };
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


        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("app_name").Equals("QA") && HttpContext.Session.GetString("role_name").Equals("Admin") || HttpContext.Session.GetString("role_name").Equals("Super Admin") || HttpContext.Session.GetString("app_name").Equals("QA"))
            {
                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Columns = new List<String>
                    {
                        "serial_no",
                        "visit_no",
                        "logged_date",
                        "division",
                        "department",
                        "status",
                        "site",
                        "location",
                        "nature_of_work",
                        "type_of_observation",
                        "type_of_confirmance",
                        "nature_of_confirmance",
                        "standard",
                        "basics",
                        "vendor_code",
                        "vendor_name"
                    };
                    List<int> departmentIds = new();
                    List<int> divisionIds = new();
                    try
                    {
                        if (TempData["departmentIds"] != null)
                        {
                            departmentIds = JsonConvert.DeserializeObject<List<int>>(TempData["departmentIds"].ToString());
                            TempData["departmentIds"] = JsonConvert.SerializeObject(departmentIds);
                        }
                        if (TempData["divisionIds"] != null)
                        {
                            divisionIds = JsonConvert.DeserializeObject<List<int>>(TempData["divisionIds"].ToString());
                            TempData["divisionIds"] = JsonConvert.SerializeObject(divisionIds);
                        }
                    }
                    catch
                    {

                    }

                    if (departmentIds.Count > 0)
                    {
                        var dept = _context.Department.Where(x => departmentIds.Contains(x.Id)).ToList();

                        ViewBag.viewDepartments = dept;
                    }
                    if (divisionIds.Count > 0)
                    {
                        var longDivisionIds = divisionIds.Select(x => (long)x).ToList();
                        var div = _context.Divisions.Where(x => longDivisionIds.Contains(x.id)).ToList();

                        ViewBag.viewDivisions = div;
                    }

                    ViewBag.Title = "View QA Observations";


                    return View();
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            else
                return RedirectToAction("Login", "Account");

        }



        [ActionName("Find")]
        public IActionResult Find(string column, string query, int page = 0, List<long> departmentIds = null, List<long> divisionIds = null)
        {
            TempData["column"] = column;
            TempData["query"] = query;
            TempData["departmentIds"] = JsonConvert.SerializeObject(departmentIds);
            TempData["divisionIds"] = JsonConvert.SerializeObject(divisionIds);
            return RedirectToAction("Index");
        }

    }
}