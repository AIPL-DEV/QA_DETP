using DataAnnotationsExtensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DETP.Controllers
{
    
    public class JobStop
    {
        [Key]
        public long Id { get; set; }
        [Email]
        public string Email { get; set; }
    }

    public class JobStoppedEmailController : Controller
    {

        sqlhelp sqlhelp = new sqlhelp();
        public IActionResult Index()
        {
            sqlhelp.fetch1("SELECT * FROM job_stop_email");

            List<JobStop> jobStops = new();

            foreach (DataRow item in sqlhelp.datatable1.Rows)
            {
                jobStops.Add(new JobStop()
                {
                    Id = long.Parse(item.ItemArray[0].ToString()),
                    Email = (string)item.ItemArray[1]
                });
            }

            return View(jobStops);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost, ActionName("Create")]

        public IActionResult Create(JobStop jobStop)
        {
            if(jobStop != null && jobStop.Email != null)
            {
                sqlhelp.performAction($"INSERT INTO job_stop_email(email) VALUES('{jobStop.Email}')");
                return RedirectToAction("Index");
            }
            return View(jobStop);
        }

        public IActionResult Delete(long Id)
        {

            sqlhelp.performAction($"DELETE FROM job_stop_email WHERE id={Id}");
            return RedirectToAction("Index");
            
        }

        [HttpGet]
        public IActionResult Edit(long Id)
        {
            var jobStop = new JobStop();
            sqlhelp.fetch1($"SELECT * FROM job_stop_email WHERE id={Id}");
            if(sqlhelp.datatable1.Rows.Count > 0){
                jobStop.Id = (long)sqlhelp.datatable1.Rows[0].ItemArray[0];
                jobStop.Email = (string)sqlhelp.datatable1.Rows[0].ItemArray[1];
            }
            return View(jobStop);
        }

        [HttpPost]
        public IActionResult Edit(long Id, JobStop jobStop)
        {
            sqlhelp.fetch1($"SELECT * FROM job_stop_email WHERE id={Id}");
            if (sqlhelp.datatable1.Rows.Count > 0)
            {
                sqlhelp.performAction($"UPDATE job_stop_email SET email='{jobStop.Email}' WHERE id={Id}");
            }
            return RedirectToAction("Index");
        }


    }
}
