using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SSO_SecurityServerF.Mailer
{
    public class Helpers
    {
        public static string RandomString(int size)
        {
            var chars = "0123456789";
            var random = new Random();
            var result = new string(
                System.Linq.Enumerable.Repeat(chars, size)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }

        public static string GetConfig(string val)
        {
            IConfigurationRoot Configuration;

            string jsonPhat = System.IO.Directory.GetCurrentDirectory();
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            directory=directory.Replace("SSO-UPCI", "SSO-SecurityServerF");
            jsonPhat = directory + "\\mailer";

            var builder = new ConfigurationBuilder()
            .SetBasePath(jsonPhat)
            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            return Configuration[val];
        }

        public static bool IsValidMail(string emailaddress)
        {
            if (String.IsNullOrEmpty(emailaddress)) return false;
            emailaddress = emailaddress.Trim();

            try
            {
                var m = new System.Net.Mail.MailAddress(emailaddress);
                var regex = new System.Text.RegularExpressions.Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var match = regex.Match(emailaddress);
                if (match.Success) return true;
                else return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string isValidMobile(string cel)
        {
            if (String.IsNullOrEmpty(cel)) return null;
            var rgx = new System.Text.RegularExpressions.Regex("[^0-9]");
            var res = rgx.Replace(cel, "");
            if (res.Length < 9) return null;
            else
            {
                res = res.Substring(res.Length - 9);
                if (res.IndexOf("9") == 0) return res;
                else return null;
            }
        }

        public static string truncate(string val, int length)
        {
            if (val == null || val.Length <= length) return val;
            else
            {
                return val.Substring(0, length);
            }
        }

        public static string ENV()
        {
            string Amb = GetConfig("AMB:Ambiente");
            return Amb.ToUpper();
        }

        public static string dominio()
        {
            string _domain = "localhost:44370";
            return _domain.ToUpper();
        }
        public static bool PROD()
        {
            return ENV().ToUpper() == "PRO" ? true : false;
        }

        public static string LeerConfig(string _config)
        {
            string _Result = GetConfig(_config); // _config podrá ser "DOMINIO:Host" ó "AMB:Ambiente" ó "MESSAGES:0", etc...
            return _Result;
        }
    }
}
