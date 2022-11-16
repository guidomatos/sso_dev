using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using SSO_BusinessLogic;
using SSO_BusinessLogic.Interfaces;
using SSO_IdentityServerF;
using SSO_Modelo.Clases;
using SSO_Modelo.Interfaces;
using SSO_SecurityServerF;
using SSO_SecurityServerF.Mailer;

namespace SSO_UPCI.Controllers.Injector
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private StandardKernel Kernel;
        public NinjectControllerFactory()
        {
            Kernel =  new StandardKernel();
            Kernel.Load(Assembly.GetExecutingAssembly());
        }

        public T Resolver<T>()
        {
            return Kernel.Get<T>();
        }

        protected override IController GetControllerInstance(RequestContext requestContext,
            Type controllerType)
        {
            return controllerType == null
                ? null
                : (IController)Kernel.Get(controllerType);
        }

        private void AddBindings()
        {
            Kernel.Bind<IConexion>().To<Conexion>();
            Kernel.Bind<ITokenGenerator>().To<TokenGenerator>();
            Kernel.Bind<IVerifyToken>().To<VerifyToken>();
            Kernel.Bind<IEncriptador>().To<Encriptador>();
            Kernel.Bind<IProcessLogic>().To<ProcessLogic>();
            //Kernel.Bind<IHelpers>().To<Helpers>();
            //Kernel.Bind<ITokenAuth>().To<TokenAuth>();

        }
    }
}
