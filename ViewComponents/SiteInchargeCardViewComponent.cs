using DETP.model;
using DETP.ViewComponents.Models;
using Microsoft.AspNetCore.Mvc;

namespace DETP.ViewComponents
{
    public class SiteInchargeCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(SiteIncharge siteIncharge, int index, string roleName)
        {

            return View(
                new SiteInchargeCardModel
                {
                    SiteIncharge = siteIncharge,
                    Index = index,
                    IsSuperAdmin = roleName == "Super Admin"
                });
        }
    }
}
