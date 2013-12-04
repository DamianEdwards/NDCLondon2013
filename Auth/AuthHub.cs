using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace WebApplication22
{
    public class AuthHub : Hub
    {
        public override Task OnConnected()
        {
            var userName = Context.User.Identity.Name;

            var msg = "Hello " + userName + ".";
            if (Context.User.IsInRole("Admin"))
            {
                msg += " You are an Admin!";
            }
            Clients.User(userName).helloWorld(msg);

            return base.OnConnected();
        }

        [Authorize(Roles = "Admin")]
        public void DoSecretStuff()
        {

        }
    }
}