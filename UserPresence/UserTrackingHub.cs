using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace UserPresence
{
    [Authorize]
    [HubName("userTracking")]
    public class UserTrackingHub : Hub
    {
        public override async Task OnConnected()
        {
            var name = Context.User.Identity.Name;

            using (var db = new UserContext())
            {
                var user = db.Users
                    .Include(u => u.Connections)
                    .SingleOrDefault(u => u.UserName == name);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = name,
                        Connections = new List<Connection>()
                    };

                    db.Users.Add(user);
                }

                user.Connections.Add(new Connection
                {
                    ConnectionId = Context.ConnectionId,
                    UserAgent = Context.Request.Headers["User-Agent"],
                    LastActivity = DateTimeOffset.UtcNow
                });

                await db.SaveChangesAsync();
            }
        }

        public override async Task OnReconnected()
        {
            using (var db = new UserContext())
            {
                var connection = await db.Connections.FindAsync(Context.ConnectionId);

                // Ensure the connection exists
                if (connection == null)
                {
                    db.Connections.Add(new Connection
                    {
                        ConnectionId = Context.ConnectionId,
                        UserAgent = Context.Request.Headers["User-Agent"],
                        LastActivity = DateTimeOffset.UtcNow,
                        UserName = Context.User.Identity.Name
                    });
                }
                else
                {
                    connection.LastActivity = DateTimeOffset.UtcNow;
                }

                await db.SaveChangesAsync();
            }
        }

        public override async Task OnDisconnected()
        {
            using (var db = new UserContext())
            {
                var connection = await db.Connections.FindAsync(Context.ConnectionId);
                db.Connections.Remove(connection);
                await db.SaveChangesAsync();
            }
        }
    }
}