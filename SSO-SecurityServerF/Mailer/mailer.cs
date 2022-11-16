using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace SSO_SecurityServerF.Mailer
{
    public class mailer
    {


        public string body;

        public bool send(string mail_Pwd, string para, string asunto, string de, string deName, string cc, bool footer = true, string template = null)
        {
            bool snd = false;

            MailMessage mail = new MailMessage();
            if (String.IsNullOrEmpty(de))
            {
                de = $"{ConfigurationManager.AppSettings["name"]} <{ConfigurationManager.AppSettings["mail"]}>";
                mail.From = new MailAddress(de);
            }
            else
            {
                mail.From = new MailAddress(deName + " <" + de + ">");
                mail.ReplyToList.Add(new MailAddress(de));
            }

            if (!String.IsNullOrEmpty(cc))
            {
                var ccs = cc.Split(';');
                foreach (var c in ccs) if (!String.IsNullOrEmpty(c)) mail.CC.Add(new MailAddress(c));
            }


            mail.To.Add(new MailAddress(para));
            mail.Subject = asunto;
            mail.Priority = MailPriority.High;

            if (String.IsNullOrEmpty(template))
            {
                mail.Body = "<html><style type='text/css'>body,.def {font-family: Tahoma, Geneva, Verdana, sans-serif;font-size: 14px}a{text-decoration:none}</style><body>"
                    + body
                    + $"<br><br>Saludos cordiales,<br>{ConfigurationManager.AppSettings["name"]}<br>https://micuenta.upc.edu.pe"
                    + "</body></html>";
            }
            else
            {
                mail.Body = template;
            }

            mail.IsBodyHtml = true;

            //try
            //{
            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["host"], Convert.ToInt32(587));
            NetworkCredential credentials = new NetworkCredential(ConfigurationManager.AppSettings["user"], mail_Pwd, ConfigurationManager.AppSettings["domain"]);

            client.Credentials = credentials;
            client.EnableSsl = true;

            client.Send(mail);
            snd = true;
            //}
            //catch (Exception ex)
            //{
            //    snd = false;
            //}
            //finally
            //{
            //    mail = null;
            //}

            return snd;
        }

        public bool mail_tpl_reset_pass(string mail_Pwd, string nombre, string email, string code, string domain, string copyr)
        {
            string HTML = "";
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            //directory = directory.Replace("SSO-UPCI", "SSO-SecurityServerF");
            string templatePhat = directory + "\\Content\\templates\\emailReset.html";

            using (var sr = new System.IO.StreamReader(templatePhat))
            {
                HTML = sr.ReadToEnd();
            }

            //replace
            HTML = HTML.Replace("{nombre}", nombre);
            HTML = HTML.Replace("{code}", code);
            HTML = HTML.Replace("{domain}", domain);
            HTML = HTML.Replace("{copyr}", copyr);

            return send(mail_Pwd, email, "UPC - Recuperar contraseña", null, null, null, false, HTML);
        }

        public bool mail_confirm(string mail_Pwd, string nombre, string email, string code, string domain, string copyr)
        {
            string HTML = "";
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            string templatePhat = directory + "\\Content\\templates\\emailConfirm.html";

            using (var sr = new System.IO.StreamReader(templatePhat))
            {
                HTML = sr.ReadToEnd();
            }

            //replace
            HTML = HTML.Replace("{nombre}", nombre);
            HTML = HTML.Replace("{code}", code);
            HTML = HTML.Replace("{domain}", domain);
            HTML = HTML.Replace("{copyr}", copyr);

            return send(mail_Pwd, email, "UPC - Actualización de datos", null, null, null, false, HTML);
        }

    }
}
