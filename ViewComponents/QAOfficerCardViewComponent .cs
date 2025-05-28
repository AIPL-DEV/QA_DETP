using DETP.model;
using DETP.ViewComponents.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DETP.ViewComponents
{
    public class QAOfficerCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(QAOfficer qAOfficer, int index, string roleName)
        {

            return View(
                new QAOfficerCardModel
                {
                    QAOfficer = qAOfficer,
                    Index = index,
                    IsSuperAdmin = roleName == "Super Admin"
                });
        }
    }
}
