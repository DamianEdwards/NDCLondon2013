using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(UserPresence.Startup))]

namespace UserPresence
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Authentication
            app.Use(async (context, next) =>
            {
                var userName = context.Request.Query["u"];

                if (!String.IsNullOrEmpty(userName))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userName)
                    };

                    context.Request.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "QueryString"));
                }

                await next();
            });

            var heartBeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();

            var monitor = new PresenceMonitor(heartBeat);
            monitor.StartMonitoring();

            app.MapSignalR();
        }
    }
}
