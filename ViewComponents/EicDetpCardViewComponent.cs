using DETP.model;
using DETP.ViewComponents.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;

namespace DETP.ViewComponents
{
    public class EicDetpCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(EicDetp eicDetp, int index, string roleName)
        {
            
            return View(
                new EicDetpCardModel
                {
                    EicDetp = eicDetp,
                    Index = index,
                    IsSuperAdmin = roleName == "Super Admin",
                    IsHoDDETP = IsHoDDETP(eicDetp.DecisionDate),
                });
        }

        public bool IsHoDDETP (DateTime? date)
        {
            if (date == null)
            {
                return false;
            }
            if (date < new DateTime(2022, 12, 03))
                return false;
            else
            {
                return true;
            }
        }
    }
}
