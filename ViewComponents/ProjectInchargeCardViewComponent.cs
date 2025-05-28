using DETP.model;
using DETP.ViewComponents.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DETP.ViewComponents
{
    public class ProjectInchargeCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProjectIncharge projectIncharge, int index, string roleName)
        {

            return View(
                new ProjectInchargeCardModel
                {
                    ProjectIncharge = projectIncharge,
                    Index = index,
                    IsSuperAdmin = roleName == "Super Admin"
                });
        }
    }
}
