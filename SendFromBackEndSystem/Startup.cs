using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Newtonsoft.Json;
using Owin;

[assembly: OwinStartup(typeof(SendFromBackEndSystem.Startup))]

namespace SendFromBackEndSystem
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/backend", map =>
            {
                map.Run(async context =>
                {
                    string body;
                    using (var reader = new StreamReader(context.Request.Body))
                    {
                        body = await reader.ReadToEndAsync();
                    }
                    dynamic payload = JsonConvert.DeserializeObject(body);
                    string message = payload.message;
                    
                    var hub = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
                    hub.Clients.All.newMessage(message);
                });
            });

            app.MapSignalR();
        }
    }
}
