using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.ServiceModel;
using System.Text;

// NOTA: puede usar el comando "Cambiar nombre" del menú "Refactorizar" para cambiar el nombre de interfaz "IIntegrationSSO" en el código y en el archivo de configuración a la vez.
[ServiceContract]
public interface IIntegrationSSO
{
    ClaimsPrincipal AUTH(string token);

    string GetTokenSSO();
    void SetTokenCookie(string token);

    [OperationContract]
    bool TokenCheker(string token);

    void KillToken();

    //[OperationContract]
    //IEnumerable<Claim> GetTokenValue(string token);

    [OperationContract]
    string GetTokenValueSeri(string token);
}
