using DETP.model;
using DETP.ViewComponents.Models;
using Microsoft.AspNetCore.Mvc;

namespace DETP.ViewComponents
{
    public class AssigneeCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Assignee assignee, int index, string roleName)
        {

            return View(
                new AssigneeCardModel
                {
                    Assignee = assignee,
                    Index = index,
                    IsSuperAdmin = roleName == "Super Admin"
                });
        }
    }
}
