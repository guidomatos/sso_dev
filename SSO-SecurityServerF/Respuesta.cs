using SSO_SecurityServerF.Clases;
using System;

namespace SSO_SecurityServerF
{
    public class Respuesta
    {
        public bool ok { get; set; }
        //JUAN CARLOS RODRIGUEZ DONAYRE 03-05-2021 (agregado)
        public int code { get; set; }  //devuelve codigos de AD del 0 al 7
        //FIN
        public string mensaje { get; set; }
        public UsuarioAD obj { get; set; }
        public JwtClaims objJwtClaims { get; set; }
        public bool noMostrar { get; set; }
        public string federada { get; set; }
        //INICIO(29-10-2020)
        public Boolean flagRecPas { get; set; }
        //FIN
        //INICIO (12-11-2020)
        public string CodFederada { get; set; }
        //FIN
        //JUAN CARLOS RODRIGUEZ DONAYRE 06-05-2021 (agregado)
        public string federada_api { get; set; }   // metodo consulta_federada, guarda A:Azure O:On premises 
        //FIN
        //juan carlos rodriguez donayre 29-09-2021 (agregado) para Actualizacion de datos
        public string celular { get; set; }
        public string correo { get; set; }
        //fin
    }
}
