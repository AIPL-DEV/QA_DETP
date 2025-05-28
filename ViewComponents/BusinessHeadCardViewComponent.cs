using DETP.model;
using DETP.ViewComponents.Models;
using Microsoft.AspNetCore.Mvc;

namespace DETP.ViewComponents
{
    public class BusinessHeadCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(BusinessHead businessHead, int index, string roleName)
        {

            return View(
                new BusinessHeadCardModel
                {
                    BusinessHead = businessHead,
                    Index = index,
                    IsSuperAdmin = roleName == "Super Admin"
                });
        }
    }
}
