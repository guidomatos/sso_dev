namespace SSO_SecurityServerF.Mailer
{
    public interface IHelpers
    {
        string RandomString(int size);
        string GetConfig(string val);
        bool IsValidMail(string emailaddress);
        string truncate(string val, int length);
        string ENV();
        string dominio();
        bool PROD();
        string LeerConfig(string _config);
    }
}