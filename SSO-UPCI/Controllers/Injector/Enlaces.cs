using Ninject.Modules;
using SSO_BusinessLogic;
using SSO_BusinessLogic.Interfaces;
using SSO_IdentityServerF;
using SSO_Modelo.Clases;
using SSO_Modelo.Interfaces;
using SSO_SecurityServerF;

namespace SSO_UPCI.Controllers.Injector
{
    public class Enlaces : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IConexion>().To<Conexion>();
            this.Bind<ITokenGenerator>().To<TokenGenerator>();
            this.Bind<IVerifyToken>().To<VerifyToken>();
            this.Bind<IEncriptador>().To<Encriptador>();
            this.Bind<IProcessLogic>().To<ProcessLogic>();
        }
    }
}
