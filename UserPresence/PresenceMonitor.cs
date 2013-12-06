using System;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR.Transports;

namespace UserPresence
{
    public class PresenceMonitor
    {
        private readonly ITransportHeartbeat _heartbeat;
        private Timer _timer;
        private readonly TimeSpan _presenceCheckInterval = TimeSpan.FromSeconds(10);

        public PresenceMonitor(ITransportHeartbeat heartbeat)
        {
            _heartbeat = heartbeat;
        }

        public void StartMonitoring()
        {
            if (_timer == null)
            {
                _timer = new Timer(_ =>
                {
                    try
                    {
                        Check();
                    }
                    catch (Exception ex)
                    {
                        // Don't throw on background threads, it'll kill the entire process
                        Trace.TraceError(ex.Message);
                    }
                }, 
                null, 
                TimeSpan.Zero, 
                _presenceCheckInterval);
            }
        }

        private void Check()
        {
            using (var db = new UserContext())
            {
                // Get all connections on this node and update the activity
                foreach (var trackedConnection in _heartbeat.GetConnections())
                {
                    if (!trackedConnection.IsAlive)
                    {
                        continue;
                    }

                    Connection connection = db.Connections.Find(trackedConnection.ConnectionId);

                    // Update the client's last activity
                    if (connection != null)
                    {
                        connection.LastActivity = DateTimeOffset.UtcNow;
                    }
                    else
                    {
                        // We have a connection that isn't tracked in our DB!
                        // Debugger.Launch();
                    }
                }

                // Now check all db connections to see if there's any zombies

                // Remove all connections that haven't been updated in the last 30 seconds
                var zombies = db.Connections.Where(c =>
                    SqlFunctions.DateDiff("ss", c.LastActivity, DateTimeOffset.UtcNow) >= 30);

                // We're doing ToList() since there's no MARS support on azure
                foreach (var connection in zombies.ToList())
                {
                    db.Connections.Remove(connection);
                }

                db.SaveChanges();
            }
        }
    }
}