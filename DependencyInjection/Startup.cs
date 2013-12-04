using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin;
using Ninject;
using Owin;

[assembly: OwinStartup(typeof(DependencyInjection.Startup))]

namespace DependencyInjection
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var kernel = new StandardKernel();
            kernel.Bind<IClockService>().To<ClockService>()
                  .InSingletonScope();

            var resolver = new NinjectDependencyResolver(kernel);

            // This is kind gross but we'll improve it in the future
            kernel.Bind<IHubContext>()
                  .ToConstant(resolver.Resolve<IConnectionManager>()
                                      .GetHubContext<Clock>())
                  .WhenInjectedInto<ClockService>();

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            app.MapSignalR(new HubConfiguration
            {
                Resolver = resolver
            });
        }
    }

    public class NinjectDependencyResolver : DefaultDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType) ?? base.GetService(serviceType);
        }
    }
}
