using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HubJsProxyMinification.Startup))]

namespace HubJsProxyMinification
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(typeof(IJavaScriptMinifier), () => new AjaxMinMinifier());

            app.MapSignalR();
        }
    }
}
