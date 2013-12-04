using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace AutoRefreshOnUpdate
{
    public class Connectivity : Hub
    {
        public override Task OnConnected()
        {
            CheckVersion();

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            CheckVersion();

            return base.OnReconnected();
        }

        private void CheckVersion()
        {
            string rawVersion = Context.QueryString["version"];
            Version version;
            Version currentVersion = typeof(Connectivity).Assembly.GetName().Version;

            if (Version.TryParse(rawVersion, out version) &&
                currentVersion != version)
            {
                Clients.Caller.updateVersion(currentVersion.ToString());
            }
        }
    }
}