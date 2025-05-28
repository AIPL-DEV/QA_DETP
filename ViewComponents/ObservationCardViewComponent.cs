using DETP.model;
using DETP.ViewComponents.Models;
using Microsoft.AspNetCore.Mvc;

namespace DETP.ViewComponents
{
    public class ObservationCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(QAObservation observation, int index, string roleName)
        {

            return View(
                new ObservationCardModel
                {
                    Observation = observation,
                    Index = index,
                    IsSuperAdmin = roleName == "Super Admin",
                    RoleName = roleName
                });
        }
    }
}
