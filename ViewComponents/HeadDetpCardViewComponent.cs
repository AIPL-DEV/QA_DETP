using DETP.model;
using DETP.ViewComponents.Models;
using Microsoft.AspNetCore.Mvc;

namespace DETP.ViewComponents
{
    public class HeadDetpCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(HeadDetp headDetp, int index, string roleName, bool isCritical)
        {

            return View(
                new HeadDetpCardModel
                {
                    HeadDetp = headDetp,
                    Index = index,
                    IsSuperAdmin = roleName == "Super Admin",
                    IsCritical = isCritical
                });
        }
    }
}
