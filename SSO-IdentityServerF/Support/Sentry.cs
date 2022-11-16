using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpRaven;
using SharpRaven.Data;
using SSO_SecurityServerF.Mailer;

namespace SSO_IdentityServerF.Support
{
    public class Sentry
    {
        public static string Dns = Helpers.GetConfig($"Sentry:{Helpers.ENV()}");

        private static RavenClient _ravenClient;
        public static RavenClient RavenClient
        {
            get
            {
                if (_ravenClient != null) return _ravenClient;
                try
                {
                    _ravenClient = new RavenClient(Dns);
                }
                catch (Exception e)
                {
                    if (string.IsNullOrEmpty(Dns))
                        throw new SentryException("You have to define 'Sentry:Dns' key in your App Settings");

                    throw new SentryException("Error instancing RavenClient", e);
                }
                return _ravenClient;
            }
        }

        public static void LogSentryWriter(Exception ex)
        {
            RavenClient.Capture(new SentryEvent(ex));
        }

        [Obsolete]
        public static void LogSentryWriter(string msg)
        {
            RavenClient.CaptureMessage(new SentryMessage(msg), ErrorLevel.Info);
        }
    }
}
