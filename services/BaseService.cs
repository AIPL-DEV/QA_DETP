using DETP.data;
using DETP.model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DETP.services
{
    public class BaseService
    {
        private readonly ApplicationDbContext _context;
        private static IConfiguration _configuration;

        public static void Configure(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public BaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SaveFlowAtt(List<string> atts, int flowId)
        {
            if (atts == null) return;
            foreach (string att1 in atts)
            {
                if (att1 != null)
                {
                    _context.QaAtts.Add(new QaAtt
                    {
                        Data = att1,
                        Type = "flow",
                        TypeId = flowId
                    });
                }
            }
        }

        public void SendObservationUpdatedMail(int observationId, string subject, int mailto)
        {
            if (mailto != 0)
            {
                var user = _context.Users.Where(x => x.UserId == mailto).FirstOrDefault();
                String email_id = user.Email;
                String name = user.Name;
                string body = $"Dear Sir/Mam,   <br />&nbsp;&nbsp;&nbsp; Please Click here to see the details-->> <b><a href='{_configuration["SiteDetails:Production_URL"]}/EditObservation?serial_no=" + observationId + "&user_id=" + mailto + "' target='_blank'>VIEW OBSERVATION</a></b><br><br>Thanks,<br>QA- DETP<br>Tata Steel UISL";
                mail.SendMail(email_id, subject, body);
            }
        }
    }
}
