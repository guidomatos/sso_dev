using System;
using System.Web;
using SSO_BusinessLogic.Interfaces;
using SSO_IdentityServerF;

namespace SSO_BusinessLogic
{
    public class TokenAuth : ITokenAuth
    {
        private readonly IVerifyToken _verifyToken;

        public TokenAuth(IVerifyToken verifyToken)
        {
            _verifyToken = verifyToken;
        }

        public string GetTokenSSO()
        {
            var token = string.Empty;

            if (HttpContext.Current.Request.Cookies["tokenSSO"] != null)
            {
                token = HttpContext.Current.Request.Cookies["tokenSSO"].ToString();
            }

            return token;
        }

        public void SetTokenCookie(string token)
        {
            var aCookie = new HttpCookie("tokenSSO");
            aCookie.Expires = DateTime.Now.AddYears(10);
            aCookie.Value = token;
        }

        public bool VerifyToken()
        {
            var token = GetTokenSSO();
            var  matchToken = _verifyToken.AUTH(token);
            bool allowedToken = matchToken != null;

            return allowedToken;
        }
    }
}
