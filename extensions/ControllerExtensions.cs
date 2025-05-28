using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DETP.extensions
{
    public static class ControllerExtensions
    {
        public static int AuthUserId(this Controller controller)
        {
            return (int)controller.HttpContext.Session.GetInt32("user");
        }
    }
}
