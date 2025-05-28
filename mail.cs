using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

namespace DETP
{
    public class MailDetail
    {
        public string From { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
    }

    public class DetpMailMessage
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
        public string[] Attachments { get; set; }
    }

    public class mail
    {
        private static IConfigurationRoot configuration;
        public static void NEVER_EAT_POISON_Disable_CertificateValidation()
        {

            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    System.Net.Security.SslPolicyErrors sslPolicyErrors
                )
                {
                    return true;
                };
            var body = "" +
                "<table>" +
                "<tr>" +
                "<td>message</td>" +
                "" +
                "</tr>" +
                "</table>";
            SendMail("sagarkarn@gmail.com", "Otp for app", body);
        }

        private static IConfigurationRoot GetConfiguration2()
        {
            if (configuration != null)
            {
                return configuration;
            }

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
            return configuration;
        }

        private static string GetFromMail()
        {

            var configuration = GetConfiguration2();
            return configuration.GetValue<string>("MailDetails:from");
        }

        public static void SendMail(DetpMailMessage mailMessage)
        {


            if (mailMessage.To.EndsWith(","))
            {
                mailMessage.To = mailMessage.To[0..^1]; 
            }
            if (mailMessage.CC == null)
            {
                mailMessage.CC = Array.Empty<string>();
            }
            if (mailMessage.BCC == null)
            {
                mailMessage.BCC = Array.Empty<string>();
            }

            var configuration = GetConfiguration2();
            string from = configuration.GetValue<string>("MailDetails:from");
            MailMessage message = new(from, mailMessage.To);
            
            foreach (var item in mailMessage.CC)
            {
                message.CC.Add(item.Trim());
            }

            foreach (var item in mailMessage.BCC)
            {
                message.Bcc.Add(item.Trim());
            }

            if (mailMessage.Attachments != null)
            {
                foreach (var file in mailMessage.Attachments)
                {
                    if (file != null)
                    {
                        message.Attachments.Add(new Attachment(file));
                    }
                }
            }

            message.Subject = mailMessage.Subject;
            message.Body = mailMessage.Body;    
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            SendMail(message);

        }

        public static void SendMail(String to, String sub, String body, string cc = null, string[] attachments = null)
        {


            if (to.EndsWith(","))
            {
                to = to[0..^1];
            }
            if (cc != null)
            {
                if (cc.EndsWith(","))
                {
                    cc = cc[0..^1];
                }
            }

            var configuration = GetConfiguration2();
            string from = configuration.GetValue<string>("MailDetails:from");
            MailMessage message = new MailMessage(from, to);
            if (cc != null)
            {
                foreach (var item in cc.Split(","))
                {
                    message.CC.Add(item.Trim());
                }


            }

            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    if (file != null)
                    {
                        message.Attachments.Add(new Attachment(file));
                    }
                }
            }


            message.Subject = sub;
            message.Body = body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            SendMail(message);

        }

        

        public static void SendMail(MailMessage message)
        {


            Task.Run(() =>
            {

                string from = GetFromMail();


                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient(configuration.GetValue<string>("MailDetails:host"), configuration.GetValue<int>("MailDetails:port")); //Gmail smtp    
                NetworkCredential basicCredential1 = new(configuration.GetValue<string>("MailDetails:username"), configuration.GetValue<string>("MailDetails:password"));
                client.EnableSsl = bool.Parse(configuration.GetValue<string>("MailDetails:EnableSsl"));
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = bool.Parse(configuration.GetValue<string>("MailDetails:UseDefaultCredentials"));
                client.Credentials = basicCredential1;
                try
                {
                    client.Send(message);
                }

                catch (Exception ex)
                {
                    //throw;
                }
            });

        }
    }
}
