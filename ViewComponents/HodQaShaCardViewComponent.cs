using DETP.model;
using DETP.ViewComponents.Models;
using Microsoft.AspNetCore.Mvc;

namespace DETP.ViewComponents
{
    public class HodQaShaCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(HodQaSha hodQaSha, int index, string roleName, bool isCritical)
        {

            return View(
                new HodQaShaCardModel
                {
                    HodQaSha = hodQaSha,
                    Index = index,
                    IsSuperAdmin = roleName == "Super Admin",
                    IsCritical = isCritical
                });
        }
    }
}
