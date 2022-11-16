using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.ServiceModel;
using System.Text;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

using System.Security.Cryptography;


// NOTA: puede usar el comando "Cambiar nombre" del menú "Refactorizar" para cambiar el nombre de clase "IntegrationSSO" en el código, en svc y en el archivo de configuración a la vez.
public class IntegrationSSO : IIntegrationSSO
{
    public ClaimsPrincipal AUTH(string token)
    {
        try
        {
            string SecurityKey = "QypGLUphTmNSZlVqWG4ycjV1OHgvQT9EKEcrS2JQZVM=";
            string audience = "UPC SSO";

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return null;

            var validationParameters = new TokenValidationParameters()
            {
                ValidAudience = audience,
                ValidIssuer = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(SecurityKey)),
                ValidateLifetime = false
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
        HttpContext.Current.Response.Cookies.Add(aCookie);
        //aCookie.Value = token;
    }

    public bool TokenCheker(string token)
    {
        //var token = GetTokenSSO();
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

    //public IEnumerable<Claim> GetTokenValue(string token)
    //{
    //    var matchToken = AUTH(token);
    //    bool allowedToken = matchToken != null;
    //    if (allowedToken) return matchToken.Claims;
    //    return null;
    //}


    public string GetTokenValueSeri(string token)
    {
        //Desencripta tripleDES
        string _stKey = System.Configuration.ConfigurationManager.AppSettings["stKey"].ToString();
        token = Desencripta3DES(token, _stKey);
        //Fin TripleDES

        var matchToken = AUTH(token);
        bool allowedToken = matchToken != null;
        if (allowedToken)
        {
            var resp = matchToken.Claims;

            var dictio = new Dictionary<string, string>();

            foreach (var claim in resp)
            {
                dictio.Add(claim.Type, claim.Value);
            }

            return JsonConvert.SerializeObject(dictio);

        }
        return null;
    }

    public string Desencripta3DES(string stCadena, string stKey)
    {

        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        byte[] keyhash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(stKey));
        hashmd5 = null;

        TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
        des.Key = keyhash;
        des.Mode = CipherMode.ECB;

        byte[] buff = Convert.FromBase64String(stCadena);
        string stDecrypted = ASCIIEncoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length));

        return stDecrypted;
    }

}
