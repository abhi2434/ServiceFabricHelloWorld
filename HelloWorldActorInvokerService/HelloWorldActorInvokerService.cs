using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using HelloWorldActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace HelloWorldActorInvokerService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class HelloWorldActorInvokerService : StatelessService
    {
        public HelloWorldActorInvokerService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ActorId actorId = ActorId.CreateRandom();
                var helloWorldProxy = ActorProxy.Create<IHelloWorldActor>(actorId, this.Context.CodePackageActivationContext.ApplicationName);
                
                var result = await helloWorldProxy.HelloWorld();
                var reverseResult = await helloWorldProxy.ReverseString(result);

                ServiceEventSource.Current.ServiceMessage(this.Context, "{1} - {2} -{0}", ++iterations, result, reverseResult);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
