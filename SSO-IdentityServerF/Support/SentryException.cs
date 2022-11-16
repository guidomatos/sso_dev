using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSO_IdentityServerF.Support
{
    public class SentryException : Exception
    {
        public SentryException()
        {
        }

        public SentryException(string message)
            : base(message)
        {
        }

        public SentryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
