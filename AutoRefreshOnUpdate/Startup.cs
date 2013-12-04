using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AutoRefreshOnUpdate.Startup))]

namespace AutoRefreshOnUpdate
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/version", map =>
            {
                map.Run(async context =>
                {
                    string version = typeof(Startup).Assembly.GetName().Version.ToString();
                    await context.Response.WriteAsync(version);
                });
            });

            app.MapSignalR();
        }
    }
}
