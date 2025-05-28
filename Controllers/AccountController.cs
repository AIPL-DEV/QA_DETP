using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System;
using System.Security.Cryptography;
using System.IO;
using Microsoft.Extensions.Logging;
using DETP.data;
using System.Linq;
using DETP.model;
using System.Collections.Generic;
using DETP.helpers;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.Json;
namespace DETP
{
    public class AccountController : Controller
    {

        sqlhelp s = new sqlhelp();
        public string ReturnUrl { get; set; }
        private ILogger<AccountController> _logger;
        private ApplicationDbContext _context;
		private IConfiguration _configuration; // add new
		private object _client;

		public AccountController(ILogger<AccountController> logger, ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _logger = logger;
            _context = applicationDbContext;
			_configuration = configuration;
			
		}


        public IActionResult Login()
        {

            var logged = HttpContext.Request.Cookies["loggedin"];
            var user = HttpContext.Request.Cookies["userid"];
            var key = "E546C8DF278CD5931069B522E695D4F2";
            if (user != null)
            {
                if (!user.Equals(""))
                {
                    user = DecryptString(user, key);
                }
            }
            ViewBag.log = logged;

            var random = new Random();
            var opers = new List<string> {
                "+", "-", "*", "/"
                };

            var num1 = random.Next(9);
            var num2 = random.Next(9);
            var oper = opers[random.Next(opers.Count - 1)];

            ViewBag.captchaNum1 = num1;
            ViewBag.captchaNum2 = num2;
            ViewBag.oper = oper;

            TempData["captcharesult"] = EvalUtils.Eval($"{num1} {oper} {num2}");

            if (logged != null)
            {
                if (logged.Equals("yes"))
                    if (user != null)
                    {
                        sqlhelp.fetch1("select user_id, pno, name, department,app,password from users where user_id='" + user + "'");
                        if (sqlhelp.datatable1.Rows.Count > 0)
                        {
                            string anmol = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();

                            sqlhelp.fetch2("select role_id from user_role where user_id = '" + sqlhelp.datatable1.Rows[0].ItemArray[0].ToString() + "'");
                            int role_id = int.Parse(sqlhelp.datatable2.Rows[0].ItemArray[0].ToString());
                            sqlhelp.fetch2("select role from role where role_id = '" + role_id + "'");
                            string role = sqlhelp.datatable2.Rows[0].ItemArray[0].ToString();
                            HttpContext.Session.SetInt32("user", int.Parse(sqlhelp.datatable1.Rows[0].ItemArray[0].ToString()));
                            HttpContext.Session.Set("logged_user", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[0].ToString()));
                            HttpContext.Session.Set("pno", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[1].ToString()));
                            HttpContext.Session.Set("name", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[2].ToString()));
                            HttpContext.Session.SetInt32("role", role_id);
                            HttpContext.Session.Set("role_name", Encoding.ASCII.GetBytes(role));
                            HttpContext.Session.SetInt32("department", int.Parse(sqlhelp.datatable1.Rows[0].ItemArray[3].ToString()));
                            HttpContext.Session.Set("app_name", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[4].ToString()));

                            //ReturnUrl = HttpContext.Request.Query["ReturnUrl"];
                            //if (ReturnUrl == null)
                            //{
                                return RedirectToAction("Index", "Home");
                            //}
                            //else
                            //{
                            //    return RedirectTo
                            //}
                        }
                    }
            }



            if (HttpContext.Session.Get("user") == null)
                return View();
            else
            {
                
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult SHALogin()
        {

            var logged = HttpContext.Request.Cookies["loggedin"];
            var user = HttpContext.Request.Cookies["userid"];
            var key = "E546C8DF278CD5931069B522E695D4F2";
            if (user != null)
            {
                if (!user.Equals(""))
                {
                    user = DecryptString(user, key);
                }
            }
            ViewBag.log = logged;

            var random = new Random();
            var opers = new List<string> {
                "+", "-"
                };

            var num1 = random.Next(9);
            var num2 = random.Next(9);
            var oper = opers[random.Next(opers.Count - 1)];

            ViewBag.captchaNum1 = num1;
            ViewBag.captchaNum2 = num2;
            ViewBag.oper = oper;

            TempData["captcharesult"] = EvalUtils.Eval($"{num1} {oper} {num2}");

            if (logged != null)
            {
                if (logged.Equals("yes"))
                    if (user != null)
                    {
                        sqlhelp.fetch1("select user_id, pno, name, department,app,password from users where user_id='" + user + "'");
                        if (sqlhelp.datatable1.Rows.Count > 0)
                        {
                            string anmol = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();

                            sqlhelp.fetch2("select role_id from user_role where user_id = '" + sqlhelp.datatable1.Rows[0].ItemArray[0].ToString() + "'");
                            int role_id = int.Parse(sqlhelp.datatable2.Rows[0].ItemArray[0].ToString());
                            sqlhelp.fetch2("select role from role where role_id = '" + role_id + "'");
                            string role = sqlhelp.datatable2.Rows[0].ItemArray[0].ToString();
                            HttpContext.Session.SetInt32("user", int.Parse(sqlhelp.datatable1.Rows[0].ItemArray[0].ToString()));
                            HttpContext.Session.Set("logged_user", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[0].ToString()));
                            HttpContext.Session.Set("pno", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[1].ToString()));
                            HttpContext.Session.Set("name", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[2].ToString()));
                            HttpContext.Session.SetInt32("role", role_id);
                            HttpContext.Session.Set("role_name", Encoding.ASCII.GetBytes(role));
                            HttpContext.Session.SetInt32("department", int.Parse(sqlhelp.datatable1.Rows[0].ItemArray[3].ToString()));
                            HttpContext.Session.Set("app_name", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[4].ToString()));
                            return RedirectToAction("Index", "Home");
                        }
                    }
            }
            if (HttpContext.Session.Get("user") == null)
                return View();
            else
                return RedirectToAction("Index", "Home");
        }

        public IActionResult MLogin()
        {

            var logged = HttpContext.Request.Cookies["loggedin"];
            var user = HttpContext.Request.Cookies["userid"];
            var key = "E546C8DF278CD5931069B522E695D4F2";
            if (user != null)
            {
                if (!user.Equals(""))
                {
                    user = DecryptString(user, key);
                }
            }
            ViewBag.log = logged;
            if (logged != null)
            {
                if (logged.Equals("yes"))
                    if (user != null)
                    {
                        sqlhelp.fetch1("select user_id, pno, name, department,app,password from users where user_id='" + user + "'");
                        if (sqlhelp.datatable1.Rows.Count > 0)
                        {
                            string anmol = sqlhelp.datatable1.Rows[0].ItemArray[0].ToString();

                            sqlhelp.fetch2("select role_id from user_role where user_id = '" + sqlhelp.datatable1.Rows[0].ItemArray[0].ToString() + "'");
                            int role_id = int.Parse(sqlhelp.datatable2.Rows[0].ItemArray[0].ToString());
                            sqlhelp.fetch2("select role from role where role_id = '" + role_id + "'");
                            string role = sqlhelp.datatable2.Rows[0].ItemArray[0].ToString();
                            HttpContext.Session.SetInt32("user", int.Parse(sqlhelp.datatable1.Rows[0].ItemArray[0].ToString()));
                            HttpContext.Session.Set("logged_user", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[0].ToString()));
                            HttpContext.Session.Set("pno", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[1].ToString()));
                            HttpContext.Session.Set("name", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[2].ToString()));
                            HttpContext.Session.SetInt32("role", role_id);
                            HttpContext.Session.Set("role_name", Encoding.ASCII.GetBytes(role));
                            HttpContext.Session.SetInt32("department", int.Parse(sqlhelp.datatable1.Rows[0].ItemArray[3].ToString()));
                            HttpContext.Session.Set("app_name", Encoding.ASCII.GetBytes(sqlhelp.datatable1.Rows[0].ItemArray[4].ToString()));
                            return RedirectToAction("Index", "Home");
                        }
                    }
            }
            if (HttpContext.Session.Get("user") == null)
                return View();
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Validate(string pno, string password, string app, string rem, string captcha)
        {
            return Authapi("web", pno, password, app, rem, captcha);
        }

        public RedirectToActionResult LogOut()
        {

            var isQa = false;

            if (HttpContext.Session.GetString("app_name").Equals("QA"))
            {
                isQa = true;
            }

            HttpContext.Response.Cookies.Append("loggedin", "no");
            HttpContext.Response.Cookies.Append("userid", "");
            HttpContext.Session.Clear();
            if (!isQa)
            {
                return RedirectToAction("SHALogin", "Account");

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public ActionResult Forgot(string pno)
        {
            try
            {
                
                User user = _context.Users.Where(x=> x.PNo == pno).FirstOrDefault();

                Random r = new();
                int n = r.Next();
                var password = md5.encryption(n.ToString());
                user.Password = password;
                _context.SaveChanges();
                String body = "Dear <i>" + user.Name + "</i>,<br><br>Please find the new reset password :-<b>" + n + "</b><br><br><u>Note :- We request you to kindly copy & paste the password.</u><br><br>Thanks,<br>QA- DETP<br>Tata Steel UISL";
                mail.SendMail(user.Email, "Reset Password of QA Application", body);
                if (!user.Email.Equals(""))
                {
                    return Json(new { status = true, message = "success" });
                }

            }
            catch(Exception e) {
                _logger.LogError(e.StackTrace);
            }
            return Json(new { status = true, message = "success" });
        }
        public static string EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string DecryptString(string cipherText, string keyString)
        {
            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);

                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - iv.Length];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);

                var key = Encoding.UTF8.GetBytes(keyString);

                using (var aesAlg = Aes.Create())
                {
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        string result;
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    try
                                    {
                                        result = srDecrypt.ReadToEnd();
                                    }
                                    catch
                                    {
                                        result = "0";

                                    }
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch
            {
                
                string result = "0";
                return result;

            }
        }
        [HttpPost]
        public JsonResult Authapi(string requestFrom, string pno, string password, string app, string rem, string captcha)
        {
            //try
            //{
                var key = "E546C8DF278CD5931069B522E695D4F2";
                password = md5.encryption(password.ToString());

                string captchaResult = TempData["captcharesult"]?.ToString();

                if (!string.IsNullOrEmpty(captchaResult))
                {
                    if(captcha != captchaResult)
                    {
                        return Json(new { status = false, message = "Incorrect Captcha" });
                    }
                }

			// api

			var url = _configuration["apiurl"];
			var headerKey = _configuration["apikeyvalue"];
            var headerValue = "XApiKey";  // header value in variable
			var domain = _configuration["Domain"];
			var payload = new
			{
				PNo = pno,
				Password = password,
                Domain = domain,
			};


			string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
			var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

			using var client = new HttpClient();

			// Add header with header name and value
			client.DefaultRequestHeaders.Add(headerValue, headerKey);

			// Now send POST request
			var response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode == false)
            {

                return Json(new { status = false, message = "ADID Credentials Wrong" });

             //   string error = response.Content.ReadAsStringAsync().Result;
                // Log or inspect error details

                // Handle error accordingly
            }
            else
            {













                var user = _context.Users.Include(x => x.Department).Where(x => x.PNo == pno  && x.App == app).FirstOrDefault();

                if (user != null)
                {

                    //sqlhelp.fetch2("select role_id from user_role where user_id = '" + sqlhelp.datatable1.Rows[0].ItemArray[0].ToString() + "'");
                    var userRoles = _context.UserRoles.Where(x => x.UserId == user.UserId).FirstOrDefault();

                    var role = _context.Roles.Where(x => x.Id == userRoles.RoleId).FirstOrDefault();
                    //sqlhelp.fetch2("select role from role where role_id = '" + userRoles.RoleId + "'");
                    //string role = sqlhelp.datatable2.Rows[0].ItemArray[0].ToString();
                    HttpContext.Session.SetInt32("user", user.UserId);
                    HttpContext.Session.Set("logged_user", Encoding.ASCII.GetBytes(user.UserId.ToString()));
                    HttpContext.Session.Set("pno", Encoding.ASCII.GetBytes(user.PNo));
                    HttpContext.Session.Set("name", Encoding.ASCII.GetBytes(user.Name));

                    if (role != null)
                    {

                        HttpContext.Session.SetInt32("role", role.Id);
                        HttpContext.Session.Set("role_name", Encoding.ASCII.GetBytes(role.Name));
                    }
                    HttpContext.Session.SetInt32("department", user.Department?.Id ?? 0);
                    HttpContext.Session.Set("app_name", Encoding.ASCII.GetBytes(user.App));


                    var ip = HttpContext.Connection.RemoteIpAddress;
                    _context.LoginLogs.Add(new LoginLog
                    {
                        UserId = user.UserId,
                        Ip = ip.ToString(),
                        Datetime = DateTime.Now,
                    });
                    //sqlhelp.insert("login_logs", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), sqlhelp.datatable1.Rows[0].ItemArray[1].ToString(), ip.ToString());
                    if (rem.Equals("1"))
                    {
                        CookieOptions option = new CookieOptions();
                        option.Expires = DateTime.Now.AddDays(365);

                        var userid = EncryptString(user.UserId.ToString(), key);

                        HttpContext.Response.Cookies.Append("loggedin", "yes", option);
                        HttpContext.Response.Cookies.Append("userid", userid, option);

                    }
                    return Json(new { status = true, message = "success" });
                }
                else
                {
                    return Json(new { status = false, message = "Wrong Credentials" });

                }
            }
            //}
            //catch (Exception e)
            //{
            //    _logger.LogError(e.StackTrace);
            //    return Json(new { status = false, message = e.InnerException.Message });
            //}
        }

        [HttpGet]
        public string RefreshCaptcha()
        {
            var random = new Random();
            var opers = new List<string> { "+", "-" };

            var num1 = random.Next(9);
            var num2 = random.Next(9);
            var oper = opers[random.Next(opers.Count - 1)];

            var text = $"{num1} {oper} {num2}";

            TempData["captcharesult"] = EvalUtils.Eval(text);



            Bitmap bitmap =  TextToImage.convert(text.Replace("*", "X"));
            return TextToImage.ToBase64(bitmap);
            
        }
    }
}