namespace SSO_Modelo.DTO
{
    public class AuthenticationOnAzureResponseDto
    {
        public bool ok;
        public string msg;
        public int code;
        public AuthenticationOnAzureUserDetailResponseDto AdUser { get; set; }
    }
    public class AuthenticationOnAzureUserDetailResponseDto
    {
        public string Email { get; set; }
        public string EmailUPC { get; set; }
        //public string EmailPersonal { get; set; }
        public string Anexo { get; set; }
        //public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        //public string CN { get; set; }
        //public string OU { get; set; }
        //public string SamAcountName { get; set; }
    }
}