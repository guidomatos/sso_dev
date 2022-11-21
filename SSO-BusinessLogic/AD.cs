using System;
using System.Configuration;
using System.ServiceModel;
using System.Threading.Tasks;
using SSO_SecurityServerF.Clases;

namespace SSO_BusinessLogic
{
    public class AD
    {
        readonly WSAD.ServiceClient ws;
        //readonly string wsad = "http://localhost:56616/Service.svc";
        //readonly string wsad = ConfigurationManager.AppSettings["wsad"];
        public class UserAD : AdUser
        {
            public string EmailPersonal { get; set; }
            public string Cel { get; set; }
        }
        public AD()
        {
            var wsad = string.Empty;
            switch (ConfigurationManager.AppSettings["ENV"])
            {
                case "DEV":
                    wsad = ConfigurationManager.AppSettings["wsad"];
                    break;
                case "CRT":
                    wsad = ConfigurationManager.AppSettings["wsac"];
                    break;
                case "PRD":
                    wsad = ConfigurationManager.AppSettings["wsap"];
                    break;
            }
            ws = new WSAD.ServiceClient(GetBinding(), new EndpointAddress(wsad));
        }
        public async Task<(bool ok, string msg, int code)> Auth(string user, string pass)
        {
            try
            {
                //#if DEBUG
                //                return (true, "", 0);
                //#endif

                System.Net.ServicePointManager.DefaultConnectionLimit = 900000;

                var r = ws.AuthAsync(user, pass);

                return (r.Result.ok, r.Result.msg, r.Result.code);

            }
            catch // (Exception x)
            {
                //if (!Helpers.PROD()) Sentry.LogSentryWriter(x);

                return (false, "Lo sentimos, tuvimos un error en la comunicación. Por favor intentar nuevamente.", 101);
            }
        }
        public async Task<resAD> GetUser(string user)
        {
            try
            {
                var adUser = ws.GetUserAsync(user);
                var result = new resAD();
                if (adUser.Result.AdUser != null)
                {
                    var userAd = new UserAD()
                    {
                        Id = user,
                        Name = adUser.Result.AdUser.Name,
                        LastName = adUser.Result.AdUser.LastName,
                        Email = adUser.Result.AdUser.Email ?? string.Empty,
                        EmailPersonal = adUser.Result.AdUser.Email ?? string.Empty,
                        EmailUPC = adUser.Result.AdUser.EmailUPC,
                        Phone = adUser.Result.AdUser.Phone,
                        OU = adUser.Result.AdUser.OU
                    };

                    result.ok = adUser.Result.ok;
                    result.AdUser = userAd;
                    result.code = adUser.Result.code;
                    result.msg = adUser.Result.msg;

                    return result;
                }

                result.ok = false;
                result.code = 6;
                return result;
            }

            catch
            {
                return null;
            }
        }
        public (bool, string) SetEmail(string user, string email, string cel = null)
        {
            var r = ws.SetEmailAsync(user, email);

            //Sentry.LogSentryWriter(Newtonsoft.Json.JsonConvert.SerializeObject(r));

            return (r.Result.ok, r.Result.msg);
        }
        public (bool, string) ChangePasswordAD(string user, string pwd)
        {
            var r = ws.ChangePasswordAsync(user, pwd);

            if (!r.Result.ok) r.Result.msg = "La nueva contraseña no cumple con los requisitos mínimos";

            return (r.Result.ok, r.Result.msg);
        }
        //public string GetModifiedUsers()
        //{
        //    var r = ws.ModifiedUsersAsync(DateTime.Today);
        //    return r.Result;
        //}
        private static BasicHttpBinding GetBinding()
        {
            var binding = new BasicHttpBinding
            {
                MaxBufferPoolSize = long.MaxValue,
                MaxBufferSize = int.MaxValue,
                ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                MaxReceivedMessageSize = int.MaxValue,
                AllowCookies = true
            };
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.SendTimeout = TimeSpan.FromSeconds(20);
            return binding;
        }
    }
}