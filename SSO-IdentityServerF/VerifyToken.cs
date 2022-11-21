using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using SSO_Modelo.Interfaces;

using SSO_SecurityServerF;

namespace SSO_IdentityServerF
{
    public class VerifyToken : IVerifyToken
    {
        //public string audience = "UPC SSO";
        //private string SecurityKey = "QypGLUphTmNSZlVqWG4ycjV1OHgvQT9EKEcrS2JQZVM=";
        private readonly IConexion _conexion;
        private readonly IEncriptador _encriptador;

        public VerifyToken(IConexion conexion, IEncriptador encriptador)
        {
            _conexion = conexion;
            _encriptador = encriptador;
        }

        public ClaimsPrincipal AUTH(string token)
        {
            try
            {
                string audience = _conexion.Consulta_Variables("ssoTokenAudience").mensaje;
                string SecurityKey = _conexion.Consulta_Variables("ssoSecurityKey").mensaje;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;


                var validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = audience,
                    ValidIssuer = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(SecurityKey)),
                    ValidateLifetime = true
                };

                SecurityToken securityToken;
                var validateToken = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return validateToken;
            }
            catch (Exception ex)
            {
                var aux = ex;
                return null;
            }
        }

        public string GetTokenSSO()
        {
            var token = string.Empty;

            if (HttpContext.Current.Request.Cookies["tokenSSO"] != null)
            {
                token = HttpContext.Current.Request.Cookies.Get("tokenSSO").Value;
            }

            return token;
        }

        public void SetTokenCookie(string token)
        {
            HttpCookie aCookie = new HttpCookie("tokenSSO", token);
            aCookie.Expires = DateTime.Now.AddYears(10);
            HttpContext.Current.Response.Cookies.Add(aCookie);
            //aCookie.Value = token;
        }

        public bool TokenCheker()
        {
            var token = GetTokenSSO();
            //Desencripta tripleDES
            //string _stKey = System.Configuration.ConfigurationManager.AppSettings["stKey"].ToString();
            //token = _encriptador.Desencripta3DES(token, _stKey);
            //Fin TripleDES
            var matchToken = AUTH(token);
            bool allowedToken = matchToken != null;

            return allowedToken;
        }

        public void KillToken()
        {
            if (HttpContext.Current.Request.Cookies["tokenSSO"] != null)
            {
                //HttpContext.Current.Response.Cookies.Remove("tokenSSO");
                var c = new HttpCookie("tokenSSO");
                c.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(c);
            }
        }
    }
}
