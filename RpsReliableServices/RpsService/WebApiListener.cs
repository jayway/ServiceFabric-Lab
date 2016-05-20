using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace RpsService
{
    public class WebApiListener : ICommunicationListener
    {
        private readonly Startup _startup;
        private readonly ServiceContext _context;
        private IDisposable _webApp;

        public WebApiListener(IReliableStateManager stateManager, ServiceContext context)
        {
            _startup = new Startup(stateManager);
            _context = context;
        }

        public void Abort()
        {
            _webApp.Dispose();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            _webApp.Dispose();
            return Task.FromResult(0);
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var endpointDesc = _context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");

            var listeningAddress = $"http://+:{endpointDesc.Port}/{Guid.NewGuid()}/";

            _webApp = WebApp.Start(listeningAddress, _startup.Configuration);
            var publishAddress = listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);
            return Task.FromResult(publishAddress);
        }
    }
}
