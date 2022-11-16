using System;
using System.Security.Claims;

namespace SSO_IdentityServerF
{
    public interface ITokenGenerator
    {
        Claim[] createClaims(string Sub, string GivenName, string FamilyName, string Email, string requestor,
            bool isAdmin);

        string generateToken(string Sub, string GivenName, string FamilyName, string Email, TimeSpan duration,
            string requestor, bool isAdmin);
    }
}