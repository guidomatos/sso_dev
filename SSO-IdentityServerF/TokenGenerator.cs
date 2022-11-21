using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SSO_Modelo.Interfaces;

namespace SSO_IdentityServerF
{
    public class TokenGenerator : ITokenGenerator
    {

        private readonly IConexion _conexion;

        public TokenGenerator(IConexion conexion)
        {
            _conexion = conexion;
        }


        public Claim[] createClaims(string Sub, string GivenName, string FamilyName, string Email, string requestor, bool isAdmin)
        {
            var claims = new System.Collections.Generic.List<Claim>
                   {
                        new Claim("IdUPC",Sub),
                        new Claim("Nombre",GivenName),
                        new Claim("Apellido",FamilyName),
                        new Claim("Email", Email),
                        new Claim("HoraSesion", DateTime.UtcNow.ToString()),
                        new Claim("Website",requestor),
                        new Claim(ClaimTypes.PrimarySid,Sub),
                        new Claim(ClaimTypes.Email,Email),
                        new Claim(ClaimTypes.GivenName,GivenName),
                        new Claim(ClaimTypes.Surname,FamilyName)
                   };

            if (isAdmin) claims.Add(new Claim("ROL", "ADMIN"));
            return claims.ToArray();
        }

        public string generateToken(string Sub, string GivenName, string FamilyName, string Email, TimeSpan duration, string requestor, bool isAdmin)
        {
            string audience = _conexion.Consulta_Variables("ssoTokenAudience").mensaje;
            string SecurityKey = _conexion.Consulta_Variables("ssoSecurityKey").mensaje;

            SymmetricSecurityKey SymmetricSecurityKey = new SymmetricSecurityKey(Convert.FromBase64String(SecurityKey));
            SigningCredentials SigningCredentials = new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                       issuer: audience,
                       audience: audience,
                       claims: createClaims(Sub, GivenName, FamilyName, Email, requestor, isAdmin),
                       expires: DateTime.UtcNow.Add(duration),
                       notBefore: DateTime.UtcNow,
                       signingCredentials: SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
