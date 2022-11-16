namespace SSO_BusinessLogic.Interfaces
{
    public interface ITokenAuth
    {
        string GetTokenSSO();
        void SetTokenCookie(string token);
        bool VerifyToken();
    }
}
