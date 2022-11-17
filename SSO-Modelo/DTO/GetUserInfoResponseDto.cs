using System.Collections.Generic;

namespace SSO_Modelo.DTO
{
    public class GetUserInfoResponseDto
    {
        public GetUserInfoHeaderResponseDto DTOHeader { get; set; }
        public List<GetUserResumeInfoDetailResponseDto> ListaDTOUsuarios { get; set; }
    }
    public class GetUserInfoHeaderResponseDto
    {
        public string CodigoRetorno { get; set; }
        public string DescRetorno { get; set; }
    }
    public class GetUserResumeInfoDetailResponseDto
    {
        public GetUserInfoDetailHeaderResponseDto DTOUsuarioCabecera { get; set; }
        public List<GetUserInfoDetailResponseDto> DetalleDTOUsuario { get; set; }
    }
    public class GetUserInfoDetailHeaderResponseDto
    {
        public string CodLineaNegocio { get; set; }
        public string CodUsuario { get; set; }
    }
    public class GetUserInfoDetailResponseDto
    {
        public string CodPersona { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaternoPropio { get; set; }
        public string ApellidoMaternoPropio { get; set; }
        public string NombresPropio { get; set; }
        public string TipoDocumento { get; set; }
        public string DocumentoIdentidad { get; set; }
        public string EstadoPersona { get; set; }
        public string CodTipoUsuario { get; set; }
        public string DesTipoUsuario { get; set; }
        public string Administrativo { get; set; }
        public string TipoPersona { get; set; }
        public string EstadoUsuario { get; set; }
        public string Email { get; set; }
        public string EmailAlterno { get; set; }
        public string CodArea { get; set; }
        public string CodAreaAcad { get; set; }
        public string RequiereAuditoria { get; set; }
        public string ValidaBd { get; set; }
        public string ClaveSecreta { get; set; }
        public string FechaClave { get; set; }
        public string TiempoExpira { get; set; }
        public string MaxSesiones { get; set; }
        public string IntentosFallidos { get; set; }
        public string MotivoDesactivacion { get; set; }
        public string RequierePermiso { get; set; }
        public string UsuarioLibre { get; set; }
        public string ConsultaLibre { get; set; }
        public string IndEncExaOnline { get; set; }
        public string PrimerIngreso { get; set; }
        public string FechaBloqueo { get; set; }
        public string FechaIntentos { get; set; }
        public string OrigenUpdate { get; set; }
        public string FechaCreacion { get; set; }
        public string UsuarioCreador { get; set; }
        public string FechaModificacion { get; set; }
        public string UsuarioModificador { get; set; }
        public string EsDirectorCarrera { get; set; }
        public string TelefonoMovil { get; set; }
    }
}