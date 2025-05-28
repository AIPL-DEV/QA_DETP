using DETP.data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DETP.Controllers
{
    public class RequestModel
    {
        public string date { get; set; }
        public string department { get; set; }
        public string name_of_structure { get; set; }
        public string cost_center { get; set; }
        public string priority { get; set; }
        public string contact_person_name { get; set; }
        public string contact_person_email { get; set; }
        public string contact_person_phone { get; set; }
        public string location { get; set; }
        public string structure_type { get; set; }
        public string number_of_structure { get; set; }
        public string description { get; set; }
        public IList<IFormFile> file { get; set; } = new List<IFormFile>();  
    }
    public class SHARequestController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ApplicationDbContext _context;
        public SHARequestController(IWebHostEnvironment environment, ApplicationDbContext context)
        {
            hostingEnvironment = environment;
            _context = context;
        }
        sqlhelp s = new sqlhelp();
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("app_name").Equals("SHA") && HttpContext.Session.GetString("role_name").Equals("Admin") || HttpContext.Session.GetString("role_name").Equals("Super Admin") || HttpContext.Session.GetString("app_name").Equals("SHA") && HttpContext.Session.GetString("role_name").Equals("Customer"))
            {
                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "SHA Request Generation";
                    DataTable department;
                    sqlhelp.fetch1("select department_id, department_abbr from department");
                    department = sqlhelp.datatable1;
                    Tuple<DataTable> values = new Tuple<DataTable>(department);

                    return View(values);
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            else
                return RedirectToAction("Login", "Account");
        }
        [Consumes("multipart/form-data")]
        public ActionResult Submit(
            RequestModel request)
        {
            return SubmitApi(request);
        }

        [HttpPost]
        public JsonResult SubmitApi(
            RequestModel request)
        {
            
            string job_request_no;
            try
            {
                String mm = DateTime.Now.ToString("MM");
                String yy = DateTime.Now.ToString("yyyy");
                sqlhelp.fetch1("SELECT ISNULL(MAX(sl)+1, 1) as sl FROM sha_request WHERE department='" + request.department + "' AND structure_type='" + request.structure_type + "' AND MONTH(datetime)='" + mm + "' AND YEAR(datetime)='" + yy + "'");
                String serial_no = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();
                sqlhelp.fetch1("select department_abbr from department where department_id = '" + request.department + "'");
                job_request_no = "SHA/" + sqlhelp.datatable1.Rows[0].ItemArray[0].ToString() + "/" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yy") + "/" + request.structure_type + "/" + serial_no;
                var ID = sqlhelp.insert1("sha_request", serial_no, request.date, job_request_no, request.department, request.name_of_structure, request.cost_center, request.priority, request.contact_person_name, request.contact_person_email, request.contact_person_phone, request.location, request.structure_type, request.number_of_structure, request.description, "", "", "", "", "", HttpContext.Session.GetInt32("user").ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "Pending from SHA Team");
                foreach (IFormFile formFile in request.file)
                {
                    if (formFile.Length > 0)
                    {
                        var uniqueFileName = GetUniqueFileName(formFile.FileName);
                        var uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }
                        var filePath = Path.Combine(uploads, uniqueFileName);

                        using var stream = System.IO.File.Create(filePath);
                        formFile.CopyTo(stream);
                        _context.ShaRequestImages.Add(new model.ShaRequestImage
                        {
                            Path = uniqueFileName,
                            ShaRequestId = ID,
                        });
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "fail2" });
            }
            return Json(new { status = true, message = "success", job_request_no = job_request_no });
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }
    }
}
