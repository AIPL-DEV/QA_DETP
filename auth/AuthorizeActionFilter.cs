
using Microsoft.AspNetCore.Http;
using System.Web.Mvc;

namespace DETP.auth
{
    internal class AuthorizeActionFilter : IAuthorizationFilter
    {
        private readonly string _app;
        public AuthorizeActionFilter(string app)
        {
            _app = app;
        }

        

        public void OnAuthorization(AuthorizationContext filterContext)
        {


            //HttpContext httpContext = filterContext.HttpContext;

            //bool isAuthorized = MumboJumboFunction(context.HttpContext.User, _item, _action); // :)

            //if (!isAuthorized)
            //{
            //    context.Result = new ForbidResult();
            //}
        }
    }
}