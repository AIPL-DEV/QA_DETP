using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DETP.Controllers
{
    public class EditObservationController1 : Controller
    {
        public IActionResult Index(String serial_no, int user_id)
        {
            if (HttpContext.Session.Get("user") != null)
            {
                ViewBag.Title = "Observation";

                List<KeyValuePair<String,DataTable>> values = new();
                String flow_id = "", from_id = "", status="";

                sqlhelp.fetch1("Select id,observation_id,table_name,from_id from qa_flow where observation_id = '" + serial_no + "' order by id");
                sqlhelp.fetch2("select serial_no, visit_no, logged_date, (select department_abbr from department where department_id = q.department) as department, status, site, location, nature_of_work, type_of_observation, log_non_confirmance, compliance_target_date, type_of_confirmance, nature_of_confirmance, standard, basics, job, vendor_code, vendor_name, p_no, (select name from users where user_id = q.site_incharge) as site_incharge, (select name from users where user_id = q.head_detp) as head_detp, (select name from users where user_id = q.project_incharge) as project_incharge, (select name from users where user_id = q.dept_hod) as dept_hod, (select name from users where user_id = q.business_head) as business_head, (select name from users where user_id = q.observation_by) as observation_by, observation_date  from qa_observation q where serial_no = '" + sqlhelp.datatable1.Rows[0].ItemArray[1].ToString() + "'");
                // send this observation datatable at first then rest tables
                values.Insert(0, new KeyValuePair<string, DataTable>("Observation Data", sqlhelp.datatable2));
                status = sqlhelp.datatable2.Rows[0].ItemArray[4].ToString();

                for (int i = 0; i < sqlhelp.datatable1.Rows.Count; i++)
                {
                    if (sqlhelp.datatable1.Rows[i].ItemArray[2].ToString().Equals("head_detp") && !sqlhelp.datatable1.Rows[i].ItemArray[3].ToString().Equals("5"))
                    {
                        flow_id = sqlhelp.datatable1.Rows[i].ItemArray[0].ToString();
                        from_id = sqlhelp.datatable1.Rows[i].ItemArray[3].ToString();
                        sqlhelp.fetch2("Select decision, job, remarks, (select name from users where user_id = h.decision_by) as decision_by , decision_date from head_detp h where flow_id = '" + flow_id + "'");
                        // send this datatable to cshtml file.
                        if (sqlhelp.datatable2.Rows.Count>0)
                            values.Add(new KeyValuePair<string, DataTable>("Head DETP", sqlhelp.datatable2));
                    }

                    if (sqlhelp.datatable1.Rows[i].ItemArray[2].ToString().Equals("head_detp") && sqlhelp.datatable1.Rows[i].ItemArray[3].ToString().Equals("5"))
                    {
                        flow_id = sqlhelp.datatable1.Rows[i].ItemArray[0].ToString();
                        from_id = sqlhelp.datatable1.Rows[i].ItemArray[3].ToString();
                        sqlhelp.fetch2("Select non_conformance, remarks, (select name from users where user_id = h.decision_by) as decision_by , decision_date from head_detp h where flow_id = '" + flow_id + "'");
                        // send this datatable to cshtml file.
                        if (sqlhelp.datatable2.Rows.Count > 0)
                            values.Add(new KeyValuePair<string, DataTable>("Head DETP from Business Head", sqlhelp.datatable2));
                    }

                    if (sqlhelp.datatable1.Rows[i].ItemArray[2].ToString().Equals("business_head") && sqlhelp.datatable1.Rows[i].ItemArray[3].ToString().Equals("4"))
                    {
                        flow_id = sqlhelp.datatable1.Rows[i].ItemArray[0].ToString();
                        from_id = sqlhelp.datatable1.Rows[i].ItemArray[3].ToString();
                        sqlhelp.fetch2("Select (select name from users where user_id = b.assign_to) as assign_to, target_date, remarks, (select name from users where user_id = b.decision_by) as decision_by, decision_date from business_head b where flow_id = '" + flow_id + "'");
                        if (sqlhelp.datatable2.Rows.Count > 0)
                            values.Add(new KeyValuePair<string, DataTable>("Business Head from Head DETP", sqlhelp.datatable2));
                    }

                    if (sqlhelp.datatable1.Rows[i].ItemArray[2].ToString().Equals("business_head") && sqlhelp.datatable1.Rows[i].ItemArray[3].ToString().Equals("6"))
                    {
                        flow_id = sqlhelp.datatable1.Rows[i].ItemArray[0].ToString();
                        from_id = sqlhelp.datatable1.Rows[i].ItemArray[3].ToString();
                        sqlhelp.fetch2("Select decision, remarks, (select name from users where user_id = b.decision_by) as decision_by, decision_date from business_head b where flow_id = '" + flow_id + "'");
                        if (sqlhelp.datatable2.Rows.Count > 0)
                            values.Add(new KeyValuePair<string, DataTable>("Business Head from Assignee Section", sqlhelp.datatable2));
                    }

                    if (sqlhelp.datatable1.Rows[i].ItemArray[2].ToString().Equals("assignee"))
                    {
                        flow_id = sqlhelp.datatable1.Rows[i].ItemArray[0].ToString();
                        from_id = sqlhelp.datatable1.Rows[i].ItemArray[3].ToString();
                        sqlhelp.fetch2("Select ovservation_details, root_cause_analysis, corrective_action, preventive_action, value_of_rectification, time_loss, time_value, (select name from users where user_id = a.decision_by) as decision_by, decision_date from assignee a where flow_id = '" + flow_id + "'");
                        if (sqlhelp.datatable2.Rows.Count > 0)
                            values.Add(new KeyValuePair<string, DataTable>("Assignee Section", sqlhelp.datatable2));
                    }
                }

                if(user_id == 4)
                {
                    sqlhelp.fetch2("select name from users where user_id = '" + user_id + "'");
                    DataTable dt = new DataTable();
                    dt.Clear();
                    dt.Columns.Add("Department");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("id");
                    dt.Columns.Add("from");
                    dt.Columns.Add("submit_to");
                    dt.Columns.Add("observation_id");
                    dt.Columns.Add("flow_id");
                    DataRow row = dt.NewRow();
                    row["Department"] = "Head DETP";
                    row["Name"] = sqlhelp.datatable2.Rows[0].ItemArray[0].ToString();
                    row["id"] = user_id;
                    row["from"] = from_id;
                    row["submit_to"] = "5"; // request must be submitted to Business Head
                    row["observation_id"] = serial_no;
                    row["flow_id"] = flow_id;

                    //bool found = false;
                    //foreach (var item in values)
                    //{
                    //    if (item.Key.Equals("Head DETP"))
                    //    {
                    //        found = true;
                    //        break;
                    //    }
                    //}
                    //if(!found)
                    //    dt.Rows.Add(row);
                    if (status.Equals("With Head DETP"))
                        dt.Rows.Add(row);
                    values.Add(new KeyValuePair<string, DataTable>("Current", dt));
                }
                if (user_id == 5)
                {
                    sqlhelp.fetch2("select name from users where user_id = '" + user_id + "'");
                    String userName = sqlhelp.datatable2.Rows[0].ItemArray[0].ToString();

                    sqlhelp.fetch2("select name from users where user_id = '6'");
                    String AssignToName = sqlhelp.datatable2.Rows[0].ItemArray[0].ToString();

                    DataTable dt = new DataTable();
                    dt.Clear();
                    dt.Columns.Add("Department");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("id");
                    dt.Columns.Add("from");
                    dt.Columns.Add("submit_to");
                    dt.Columns.Add("observation_id");
                    dt.Columns.Add("flow_id");
                    dt.Columns.Add("submit_to_name");
                    DataRow row = dt.NewRow();
                    row["Department"] = "Business Head";
                    row["Name"] = userName;
                    row["id"] = user_id;
                    row["from"] = from_id;
                    row["submit_to"] = "6"; // request must be submitted to Assignee Section
                    row["observation_id"] = serial_no;
                    row["flow_id"] = flow_id;
                    row["submit_to_name"] = AssignToName;
                    //bool found = false;
                    //foreach (var item in values)
                    //{
                    //    if (item.Key.Equals("Business Head"))
                    //    {
                    //        found = true;
                    //        break;
                    //    }
                    //}
                    //if (!found)
                    //dt.Rows.Add(row);
                    if (status.Equals("With Business Head"))
                        dt.Rows.Add(row);
                    values.Add(new KeyValuePair<string, DataTable>("Current", dt));
                }
                if (user_id == 6)
                {
                    sqlhelp.fetch2("select name from users where user_id = '" + user_id + "'");
                    DataTable dt = new DataTable();
                    dt.Clear();
                    dt.Columns.Add("Department");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("id");
                    dt.Columns.Add("from");
                    dt.Columns.Add("submit_to");
                    dt.Columns.Add("observation_id");
                    dt.Columns.Add("flow_id");
                    DataRow row = dt.NewRow();
                    row["Department"] = "Assignee Section";
                    row["Name"] = sqlhelp.datatable2.Rows[0].ItemArray[0].ToString();
                    row["id"] = user_id;
                    row["from"] = from_id;
                    row["submit_to"] = "5"; // request must be submitted to Business Head
                    row["observation_id"] = serial_no;
                    row["flow_id"] = flow_id;
                    bool found = false;
                    foreach (var item in values)
                    {
                        if (item.Key.Equals("Assignee Section"))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        dt.Rows.Add(row);
                    values.Add(new KeyValuePair<string, DataTable>("Current", dt));
                }

                return View(values);
            }
            else
                return RedirectToAction("Login", "Account");
        }

        public ActionResult Submit(String data, String user_id, String to, String from, String observation_id, String flow_id)
        {
            try
            {
                if (user_id.Equals("4"))
                {
                    JToken token = JObject.Parse(data);

                    if (!from.Equals("5"))
                    {
                        sqlhelp.insert("head_detp", observation_id, flow_id, token.SelectToken("decision").ToString(), token.SelectToken("job").ToString(), "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());

                        sqlhelp.insert("qa_flow", observation_id, "business_head", user_id, "5", DateTime.Today.ToString("yyyy-MM-dd")); // 5 is id of business_head
                        sqlhelp.performAction("update qa_observation set status = 'With Business Head' where serial_no = '" + observation_id + "'");
                    }
                    else
                    {
                        sqlhelp.insert("head_detp", observation_id, flow_id, "", "", token.SelectToken("decision").ToString(), token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());

                        if(token.SelectToken("decision").ToString().Equals("Ok and Closed"))
                        {
                            sqlhelp.performAction("update qa_observation set status = 'Closed by Head DETP' where serial_no = '" + observation_id + "'");
                        }
                        //more from here
                    }
                }
                if (user_id.Equals("5"))
                {
                    JToken token = JObject.Parse(data);

                    if (!from.Equals("6"))
                    {
                        sqlhelp.insert("business_head", observation_id, flow_id, token.SelectToken("assigned_to").ToString(), "", "", token.SelectToken("target_date").ToString(), token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                        
                        sqlhelp.insert("qa_flow", observation_id, "assignee", user_id, "6", DateTime.Today.ToString("yyyy-MM-dd")); // 6 is id of assignee
                        sqlhelp.performAction("update qa_observation set status = 'With Assignee Section' where serial_no = '" + observation_id + "'");
                    }
                    else
                    {
                        sqlhelp.insert("business_head", observation_id, flow_id, "4", token.SelectToken("decision").ToString(), "", "", token.SelectToken("remarks").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                        if (token.SelectToken("decision").ToString().Equals("Return to assignee"))
                        {
                            sqlhelp.insert("qa_flow", observation_id, "assignee", user_id, "6", DateTime.Today.ToString("yyyy-MM-dd")); // 6 is id of assignee
                            sqlhelp.performAction("update qa_observation set status = 'With Assignee Section' where serial_no = '" + observation_id + "'");
                        }
                        if (token.SelectToken("decision").ToString().Equals("Forward to Closer"))
                        {
                            sqlhelp.insert("qa_flow", observation_id, "head_detp", user_id, "4", DateTime.Today.ToString("yyyy-MM-dd")); // 4 is id of head detp
                            sqlhelp.performAction("update qa_observation set status = 'With Head DETP' where serial_no = '" + observation_id + "'");
                        }
                    }
                }
                if (user_id.Equals("6"))
                {
                    JToken token = JObject.Parse(data);
                    sqlhelp.insert("assignee", observation_id, flow_id, token.SelectToken("observation_details").ToString(), token.SelectToken("root_cause_analysis").ToString(), token.SelectToken("corrective_action").ToString(), token.SelectToken("preventive_action").ToString(), token.SelectToken("value_of_rectification").ToString(), token.SelectToken("time_loss").ToString(), token.SelectToken("time_value").ToString(), token.SelectToken("decision_by").ToString(), token.SelectToken("decision_date").ToString());
                    
                    sqlhelp.insert("qa_flow", observation_id, "business_head", user_id, "5", DateTime.Today.ToString("yyyy-MM-dd")); // 5 is id of business_head
                    sqlhelp.performAction("update qa_observation set status = 'With Business Head' where serial_no = '" + observation_id + "'");
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "fail" });
            }

            return Json(new { status = true, message = "success"});
        }
    }
}
