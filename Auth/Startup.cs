using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(WebApplication22.Startup))]

namespace WebApplication22
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.Map("/login", map =>
            {
                map.Run(async context =>
                {
                    var username = context.Request.Query["username"] ?? "[random]";

                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);
                    identity.AddClaim(new Claim(ClaimTypes.Name, username));
                    
                    if (String.Equals(username, "Damian", StringComparison.OrdinalIgnoreCase))
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                    }

                    context.Authentication.SignIn(identity);
                });
            });

            app.MapSignalR(new HubConfiguration { EnableDetailedErrors = true });
        }
    }
}
