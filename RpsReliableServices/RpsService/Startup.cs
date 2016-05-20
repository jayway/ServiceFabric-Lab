using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.ServiceFabric.Data;
using Owin;

namespace RpsService
{
    public class Startup
    {
        private readonly IReliableStateManager _stateManager;

        public Startup(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_stateManager);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var container = builder.Build();

            var config = new HttpConfiguration();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);

            config.EnsureInitialized();
        }
    }
}
