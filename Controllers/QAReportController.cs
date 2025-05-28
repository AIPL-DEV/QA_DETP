using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using Microsoft.Extensions.Logging;
using DETP.data;
using System.Globalization;
using DETP.model;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Web.Mvc.Routing.Constraints;
using OfficeOpenXml.Style.XmlAccess;

namespace DETP
{
    public class QAReportController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public QAReportController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string division_id, string department, string type, DateTime? startDate, DateTime? endDate)
        {
            if (HttpContext.Session.GetString("app_name").Equals("QA") || new string[] { "Super Admin", "Admin", "EIC-DETP" }.Contains(HttpContext.Session.GetString("role_name")))
            {
                
                if (HttpContext.Session.Get("user") != null)
                {
                    ViewBag.Title = "QA Observation Report";
                    DataTable departmentDataTable;
                    ViewBag.divisions = _context.Divisions.ToList();

                    sqlhelp.fetch1("select * from department");
                    departmentDataTable = sqlhelp.datatable1;
                    if (startDate != null)
                    {
                        if (endDate == null)
                        {
                            endDate = DateTime.Now;
                        }


                        var whereClause = "";

                        if (division_id != null && department == null)
                        {
                            whereClause += $" division_id in ({division_id}) AND";
                        }

                        if (department != null)
                        {
                            whereClause += $" department in ({department}) AND";
                        }
                        if (type == "critical")
                        {
                            whereClause += $" nature_of_confirmance='Critical'";
                        }
                        else
                        {
                            whereClause += $" nature_of_confirmance!='Critical'";
                        }
                        whereClause += $" AND CAST(logged_date as DATE) >= '{startDate.Value:yyyy-MM-dd}'" +
                                        $" AND CAST(logged_date as DATE) <= '{endDate.Value:yyyy-MM-dd}'" +
                                        " ORDER BY serial_no asc";


                        var column1 = getColumn(true, whereClause);
                        var data1 = getData((List<string>)column1[1], true, whereClause);

                        if (data1.Count > 0)
                        {

                            var stream = CreateReportFile(((List<string>)column1[0]).ToArray(), data1, "TataSteel UISL QA Observation Report");

                            return File(stream, "application/vnd.ms-excel", $"{startDate.Value:dd-MM-yyyy}_to_{endDate.Value:dd-MM-yyyy}.xlsx");
                        }
                        else
                        {
                            Tuple<DataTable> v = new Tuple<DataTable>(departmentDataTable);
                            ViewBag.error = "No Data Found";
                            return View(v);
                        }
                    }


                    return View();
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            else
                return RedirectToAction("Login", "Account");
        }
        private static List<object[]> GetListByDataTable(DataTable dt)
        {

            var reult = dt.AsEnumerable().ToList<object>();

            return reult.ConvertAll<object[]>(o => (object[])((DataRow)o).ItemArray);
        }

        internal static MemoryStream CreateFile(string[] column, List<object[]> data, string title)
        {

            using (ExcelPackage excel = new ExcelPackage())
            {
                string headerRange = "";
                string titleRange = "";
                string dataRange = "";

                headerRange = "A3:" + Char.ConvertFromUtf32(column.Length + 64) + "3";
                titleRange = "A1:" + Char.ConvertFromUtf32(column.Length + 64) + "1";
                dataRange = "A4:" + Char.ConvertFromUtf32(column.Length + 64) + (data.Count + 3);

                excel.Workbook.Worksheets.Add("Worksheet1");

                // Target a worksheet
                var worksheet = excel.Workbook.Worksheets["Worksheet1"];
                worksheet.Cells[headerRange].Style.Font.Bold = true;
                worksheet.Cells[headerRange].Style.Font.Size = 12;
                worksheet.Cells[headerRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[headerRange].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(1, 216, 228, 188));

                worksheet.Cells[titleRange].Merge = true;
                worksheet.Cells[titleRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1"].Value = title;
                worksheet.Cells["A1"].Style.Font.Size = 20;

                worksheet.Cells[dataRange].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[dataRange].Style.WrapText = false;
                worksheet.Cells["C:C"].Style.Numberformat.Format = "dd-mm-yyyy";
                for (int i = 1; i <= column.Length; i++)
                {
                    worksheet.Column(i).Width = 15;
                }

                worksheet.Cells[headerRange].LoadFromArrays(new List<string[]>() { column });

                worksheet.Cells[4, 1].LoadFromArrays(data);

                // Popular header row data
                var b = excel.GetAsByteArray("");
                var s = new MemoryStream(b);

                return s;
            }
        }

        


        public object[] getColumn(bool critical, string whereClause)
        {

            var table = new List<string>();

            var coulmCount = 0;
            var dateColumns = new List<int>();
            string not = "";
            if (critical)
            {
                not = "!";
            }
            string[] qaObservationColumnNames = "serial_no,visit_no,logged_date,division, department,status,site,location,nature_of_work,type_of_observation,log_non_confirmance,log_confirmance,compliance_target_date,type_of_confirmance,nature_of_confirmance,standard,basics,job,vendor_code,vendor_name,p_no,site_incharge,project_incharge,dept_hod,business_head,observation_by,observation_date,number_of_observation,area_of_concern".Split(",");
            sqlhelp.fetch1($"SELECT serial_no,visit_no,logged_date,division_id as division, department,status,site,location,nature_of_work,type_of_observation,log_non_confirmance,log_confirmance,compliance_target_date,type_of_confirmance,nature_of_confirmance,standard,basics,job,vendor_code,vendor_name,p_no,site_incharge,project_incharge,dept_hod,business_head,observation_by,observation_date,number_of_observation,area_of_concern FROM qa_observation WHERE {whereClause}");
            var observation = sqlhelp.datatable1;
            table.Add("qa_observation");

            List<string> tableNames = new();
            List<List<string>> column_name = new();
            List<List<List<string>>> data = new();



            var n = new List<string>();
            tableNames.Add("qa_observation");
            column_name.Add(qaObservationColumnNames.ToList());

            foreach (DataRow row in observation.Rows)
            {

                var qaFlows = _context.QAFlows.Where(x => x.ObservationId == int.Parse(row.ItemArray[0].ToString())).OrderBy(x=>x.Id).ToList();

                int index = 1;

                if (tableNames.Count == 1)
                {
                    foreach (QAFlow qaflow in qaFlows)
                    {

                        tableNames.Add(qaflow.TableName);
                        var name = new List<string>();

                        sqlhelp.fetch1($"SELECT * FROM {qaflow.TableName} WHERE flow_id={qaflow.Id}");

                        int skip = 0;

                        foreach (DataColumn cl_name in sqlhelp.datatable1.Columns)
                        {
                            if (skip < 3)
                            {
                                skip++;
                                continue;
                            }
                            name.Add(cl_name.ColumnName);
                        }
                        column_name.Add(name);
                    }
                }
                else
                {
                    foreach (QAFlow flow in qaFlows)
                    {

                        if (tableNames.IndexOf(flow.TableName, index) >= index)
                        {
                            index = tableNames.IndexOf(flow.TableName, index);
                        }

                        else
                        {
                            try
                            {
                                tableNames.Insert(index + 1, flow.TableName);
                                var name = new List<string>();
                                sqlhelp.fetch1($"SELECT TOP 1 * FROM {flow.TableName}");
                                int skip = 0;
                                foreach (DataColumn cl_name in sqlhelp.datatable1.Columns)
                                {
                                    if (skip < 3)
                                    {
                                        skip++;
                                        continue;
                                    }
                                    name.Add(cl_name.ColumnName);
                                }
                                column_name.Insert(index + 1, name);
                            }

                            catch (Exception e)
                            {
                                tableNames.Add(flow.TableName);

                                var name = new List<string>();
                                sqlhelp.fetch2($"SELECT TOP 1 * FROM {flow.TableName}");
                                int skip = 0;
                                foreach (DataColumn cl_name in sqlhelp.datatable2.Columns)
                                {
                                    if (skip < 3)
                                    {
                                        skip++;
                                        continue;
                                    }
                                    name.Add(cl_name.ColumnName);
                                }

                                column_name.Add(name);
                            }
                        }
                        index++;
                    }
                }
            }

            List<string> excelCol = new List<string>();

            foreach (List<string> col in column_name)
            {
                excelCol.AddRange(col);
            }
            return new object[] { excelCol, tableNames };
        }

        public List<object[]> getData(List<string> column, bool critical, string whereClause)
        {
            string not = "";
            if (critical)
            {
                not = "!";
            }

            var query = $"SELECT serial_no,visit_no,logged_date,division_id as division, department,status,site,location,nature_of_work,type_of_observation,log_non_confirmance,log_confirmance,compliance_target_date,type_of_confirmance,nature_of_confirmance,standard,basics,job,vendor_code,vendor_name,p_no,site_incharge,project_incharge,dept_hod,business_head,observation_by,observation_date,number_of_observation,area_of_concern FROM qa_observation WHERE {whereClause}";

            var obvs = sqlhelp.fetch1($"SELECT serial_no,visit_no,logged_date,division_id as division, department,status,site,location,nature_of_work,type_of_observation,log_non_confirmance,log_confirmance,compliance_target_date,type_of_confirmance,nature_of_confirmance,standard,basics,job,vendor_code,vendor_name,p_no,site_incharge,project_incharge,dept_hod,business_head,observation_by,observation_date,number_of_observation,area_of_concern FROM qa_observation WHERE {whereClause}");

            
            List<object[]> data1 = new();
            foreach (DataRow row in obvs.Rows)
            {
                List<object> unkn = new();


                for (int i = 0; i < row.ItemArray.Length; i++)
                {

                    if (obvs.Columns[i].ColumnName == "department")
                    {
                        //sqlhelp.fetch1($"SELECT department_name FROM department WHERE department_id={row.ItemArray[i]}");
                        var department = _context.Department.Where(x => x.Id == int.Parse(row.ItemArray[i].ToString())).FirstOrDefault();
                        if (department != null)
                        {
                            unkn.Add(department.Name);
                        }
                    }
                    else if (obvs.Columns[i].ColumnName == "division")
                    {
                        long id = 0;
                        if (row.ItemArray[i].ToString() != "")
                        {
                            id = long.Parse(row.ItemArray[i].ToString());
                        }
                        var division = _context.Divisions.Find(id);

                        unkn.Add(division?.name);
                    }
                    else if (new object[] { "assign_to", "site_incharge", "head_detp", "project_incharge", "dept_hod", "business_head", "qa_officer", "observation_by" }.Contains(obvs.Columns[i].ColumnName))
                    {
                        if (row.ItemArray[i].ToString() != "")
                        {

                            //sqlhelp.fetch1($"SELECT name FROM users WHERE user_id={row.ItemArray[i]}");
                            var user = _context.Users.FirstOrDefault(x => x.UserId == (int)row.ItemArray[i]);
                            if (user != null)
                            {
                                unkn.Add(user.Name);
                            }
                            else
                            {
                                unkn.Add("");
                            }
                        }
                        else
                        {
                            unkn.Add("");
                        }
                    }
                    else
                    {
                        if (row.ItemArray[i].ToString().Contains("With Head DETP"))
                        {
                            var qaFlow = _context.QAFlows.Include(x=>x.To).Where(x => x.ObservationId == int.Parse(row.ItemArray[0].ToString())).OrderBy(x=>x.Id).LastOrDefault();
                            var name = "";
                            if(qaFlow != null)
                            {
                                name = $"({qaFlow.To.Name.Trim()} - {qaFlow.To.PNo.Trim()})";
                            }
                            else
                            {
                            }
                            unkn.Add("With Sectional Head QA" + name);
                        }
                        else if (row.ItemArray[i].ToString().Contains("With EIC DETP"))
                        {
                            var qaFlow = _context.QAFlows.Include(x => x.To).Where(x => x.ObservationId == int.Parse(row.ItemArray[0].ToString())).OrderBy(x=>x.Id).LastOrDefault();
                            var name = "";
                            if (qaFlow != null)
                            {
                                name = $"({qaFlow.To.Name.Trim()} - {qaFlow.To.PNo.Trim()})";
                            }
                            unkn.Add("With HoD DETP" + name);
                        }
                        else if (row.Table.Columns[i].ToString().Equals("status"))
                        {
                            var qaFlow = _context.QAFlows.Include(x => x.To).Where(x => x.ObservationId == int.Parse(row.ItemArray[0].ToString())).OrderBy(x => x.Id).LastOrDefault();
                            var name = "";
                            if (qaFlow != null)
                            {
                                
                                name = $"({qaFlow.To?.Name?.Trim() ?? ""} - {qaFlow.To?.PNo?.Trim() ?? ""})";
                            }
                            unkn.Add(row.ItemArray[i].ToString() + name);
                        }
                        else if (obvs.Columns[i].ColumnName.ToLower().Contains("date"))
                        {
                            var value = row.ItemArray[i].ToString();
                            bool parsed = GetFormatedDate(value, out DateTime date);

                            if (parsed)
                            {
                                unkn.Add(date.Date);
                            }
                            else
                            {
                                unkn.Add(value);
                            }
                        }
                        
                        else
                        {
                            unkn.Add(row.ItemArray[i].ToString());
                        }
                    }
                }

                var qaFlows = _context.QAFlows.Where(x=> x.ObservationId == int.Parse(row.ItemArray[0].ToString())).ToList();
                sqlhelp.fetch1($"SELECT * FROM qa_flow where observation_id={row.ItemArray[0]}");
                var flow = sqlhelp.datatable1;
                int currentIndex = 0;
                foreach (QAFlow qAFlow in qaFlows)
                {
                    currentIndex++;
                    int skip = 0;
                    for (int i = currentIndex; i < column.Count; i++)
                    {
                        if (qAFlow.TableName == column[i])
                        {
                            var entity = _context.Model.FindEntityType(qAFlow.TableName);
                            sqlhelp.fetch1($"SELECT * FROM {qAFlow.TableName} where flow_id={qAFlow.Id}");
                            var table = sqlhelp.datatable1;

                            if (table.Rows.Count > 0)
                            {

                                for (int j = 0; j < table.Rows[0].ItemArray.Length; j++)
                                {
                                    if (skip < 3)
                                    {
                                        skip++;
                                        continue;
                                    }
                                    if (table.Columns[j].ColumnName == "decision_by" || table.Columns[j].ColumnName == "assign_to")
                                    {
                                        if (table.Rows[0].ItemArray[j].ToString() != "")
                                        {
                                            sqlhelp.fetch1($"SELECT name FROM users WHERE user_id={table.Rows[0].ItemArray[j]}");
                                            if (sqlhelp.datatable1.Rows.Count > 0)
                                            {
                                                unkn.Add(sqlhelp.datatable1.Rows[0].ItemArray[0]);
                                            }
                                            else
                                            {
                                                unkn.Add("");
                                            }
                                        }
                                        else
                                        {

                                            unkn.Add("");
                                        }
                                    }

                                    else if (table.Columns[j].ColumnName.ToLower().Contains("date"))
                                    {
                                        
                                        var value = table.Rows[0].ItemArray[j].ToString();
                                        bool parsed = GetFormatedDate(value, out DateTime date);

                                        if (parsed)
                                        {
                                            unkn.Add(date.Date);
                                        }
                                        else
                                        {
                                            unkn.Add(value);
                                        }
                                    }

                                    else
                                    {
                                        unkn.Add(table.Rows[0].ItemArray[j]);
                                    }
                                }
                                skip = 0;
                            }
                            currentIndex = i;
                            break;
                        }
                        else
                        {
                            sqlhelp.fetch1($"SELECT TOP 1 * FROM {column[i]}");
                            var tb = sqlhelp.datatable1;
                            if (column[i] == "qa_observation")
                            {
                                continue;
                            }

                            foreach (DataColumn item in tb.Columns)
                            {
                                if (skip < 3)
                                {
                                    skip++;
                                    continue;
                                }
                                unkn.Add("");
                            }
                            skip = 0;
                        }
                    }
                }
                currentIndex = 0;

                data1.Add(unkn.ToArray());

            }
            return data1;
        }


        internal static MemoryStream CreateReportFile(string[] column, List<object[]> data, string title)
        {

            using (ExcelPackage excel = new())
            {
                string headerRange = "";
                string titleRange = "";
                string dataRange = "";
                titleRange = "A1:" + "L1";
                var name = GetColumnRangeName(column);
                
                    int colLen = column.Length % 26;
                    if (colLen == 0) colLen = 26;
                    headerRange = "A3:" + name + "3";
                    dataRange = "A4:" + name + (data.Count + 3);
                

                excel.Workbook.Worksheets.Add("Sheet1");

                var worksheet = excel.Workbook.Worksheets["Sheet1"];
                worksheet.Cells[titleRange].Merge = true;
                worksheet.Cells[titleRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[headerRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[headerRange].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 230, 155));
                worksheet.Cells[headerRange].Style.Font.Bold = true;
                worksheet.Cells[headerRange].Style.Font.Size = 14;
                worksheet.Cells[headerRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[headerRange].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[dataRange].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells["A1"].Value = title;
                worksheet.Cells["A1"].Style.Font.Size = 20;

                for (int i = 0; i < column.Length; i++)
                {
                    if (column[i].Contains("date"))
                    {
                        string columnName = IndexToExcelColumnName(i);
                        worksheet.Cells[columnName + ":" + columnName].Style.Numberformat.Format = "dd-mm-yyyy";
                    }
                    column[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(column[i].Replace("_", " "));
                }

                for (int i = 1; i <= column.Length; i++)
                {
                    worksheet.Column(i).Width = 15;
                }

                worksheet.Cells[3, 1].LoadFromArrays(new List<string[]>() { column });
                worksheet.Cells[4, 1].LoadFromArrays(data);


                var b = excel.GetAsByteArray("");
                var s = new MemoryStream(b);

                return s;
            }
            string IndexToExcelColumnName(int index)
            {

                if (index < 0)
                {
                    throw new ArgumentException("Index must be non-negative.");
                }

                const int baseNumber = 26;
                string columnName = "";
                while (index >= 0)
                {
                    int remainder = index % baseNumber;
                    columnName = (char)(remainder + 'A') + columnName;
                    index = (index / baseNumber) - 1;
                }

                return columnName;
            }
        }

        private static string GetColumnRangeName(string[] column)
        { 
            var length = column.Length;
            var name = "";
            while(length /  26 >= 1)
            {
                name = char.ConvertFromUtf32(length % 26 + 64) + name;
                length = length / 26;
            }
            name = char.ConvertFromUtf32(length % 26 + 64) + name;
            return name;
        }

        bool GetFormatedDate(string value, out DateTime datetime)
        {
            if (DateTime.TryParse(value, out DateTime date))
            {
                datetime = date;
                return true;
            }
            else
            {
                string[] formats = { "MM/dd/yyyy", "MM/d/yyyy", "M/d/yyyy", "M/dd/yyyy", "dd/MMM/yy" };
                bool parsed = DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
                if (parsed)
                {
                    datetime = dateTime;
                    return true;
                }
                else
                {
                    datetime = DateTime.Now;
                    return false;
                }

            }
        }
    }
}