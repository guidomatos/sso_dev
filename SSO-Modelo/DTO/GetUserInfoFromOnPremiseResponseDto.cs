namespace SSO_Modelo.DTO
{
    public class GetUserInfoFromOnPremiseResponseDto
    {
        public GetUserInfoHeaderFromOnPremiseResponseDto DTOHeader { get; set; }
        public GetUserInfoDetailFromOnPremiseResponseDto DTOUsuario { get; set; }
    }
    public class GetUserInfoHeaderFromOnPremiseResponseDto
    {
        public string CodigoRetorno { get; set; }
        public string MsjRetorno { get; set; }
        public string EstadoRetorno { get; set; }
    }
    public class GetUserInfoDetailFromOnPremiseResponseDto
    {
        public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string EmailUPC { get; set; }
        public string Anexo { get; set; }
        public string CN { get; set; }
        public string OU { get; set; }
        public string NombreUsuario { get; set; }
    }
}
