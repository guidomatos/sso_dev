using System.Security.Claims;

namespace SSO_IdentityServerF
{
    public interface IVerifyToken
    {
        ClaimsPrincipal AUTH(string token);

        //TODO:Refactorizar a otra interfaz
        string GetTokenSSO();
        void SetTokenCookie(string token);
        bool TokenCheker();
        void KillToken();
    }
}