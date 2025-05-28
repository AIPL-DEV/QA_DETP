using DETP.model;
using DETP.ViewComponents.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DETP.ViewComponents
{
    public class DeptHodCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DeptHod deptHod, int index, string roleName, bool isCritical)
        {

            return View(
                new DeptHodCardModel
                {
                    DeptHod = deptHod,
                    Index = index,
                    IsCritical = isCritical,
                    IsSuperAdmin = roleName == "Super Admin",
                });
        }
    }
}
