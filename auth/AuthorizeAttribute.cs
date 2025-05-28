

using DETP.data;
using DETP.model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DETP.auth
{
    public class AuthorizeAttribute : Attribute, IResourceFilter
    {
        private readonly string[] value = Array.Empty<string>();

        public AuthorizeAttribute(string[] app = null)
        {
            this.value = app ?? Array.Empty<string>();
        }


        public AuthorizeAttribute() {  }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            try
            {
                var user = context.HttpContext.Session.GetInt32("user");
                if (user == null)
                {
                    context.Result = new RedirectToActionResult("Login", "Account", new { RedirectUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString });
                    return;
                }

                var db = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();
                IQueryable<User> query = db.Users.Where(x => x.UserId == user);
                if (!query.Any())
                {
                    context.Result = new RedirectToActionResult("Login", "Account", new { });
                    return;
                }

                foreach (var item in value)
                {
                    if (item.Contains(":"))
                    {
                        var splited = item.Split(":");
                        var app = splited.FirstOrDefault();

                        
                        var hasApp = query.Where(x => app == x.App).Any();
                        if (!hasApp)
                        {
                            context.Result = new RedirectToActionResult("PermissionError", "Home", new { });
                            return;
                        }


                        var roleList = splited[1].Split(",");
                        var dbRoles = db.Roles.Where(x => roleList.Contains(x.Name)).Select(x => x.Id).ToList();
                        var hasRole = db.UserRoles.Where(x => x.UserId == user && dbRoles.Contains(x.RoleId.Value)).Any();
                        if (!hasRole)
                        {
                            context.Result = new RedirectToActionResult("PermissionError", "Home", new { });
                            return;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { });
            }
        }
    }
}
