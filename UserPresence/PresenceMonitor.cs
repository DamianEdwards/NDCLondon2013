using System;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR.Transports;

namespace UserPresence
{
    /// <summary>
    /// This class keeps track of connections that the <see cref="UserTrackingHub"/>
    /// has seen. It uses a time based system to verify if connections are *actually* still online.
    /// Using this class combined with the connection events SignalR raises will ensure
    /// that your database will always be in sync with what SignalR is seeing.
    /// </summary>
    public class PresenceMonitor
    {
        private readonly ITransportHeartbeat _heartbeat;
        private Timer _timer;

        // How often we plan to check if the connections in our store are valid
        private readonly TimeSpan _presenceCheckInterval = TimeSpan.FromSeconds(10);

        // How many periods need pass without an update to consider a connection invalid
        private const int periodsBeforeConsideringZombie = 3;

        // The number of seconds that have to pass to consider a connection invalid.
        private readonly int _zombieThreshold;

        public PresenceMonitor(ITransportHeartbeat heartbeat)
        {
            _heartbeat = heartbeat;
            _zombieThreshold = (int)_presenceCheckInterval.TotalSeconds * periodsBeforeConsideringZombie;
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
                        // This should *NEVER* happen
                        // Debugger.Launch();
                    }
                }

                // Now check all db connections to see if there's any zombies

                // Remove all connections that haven't been updated based on our threshold
                var zombies = db.Connections.Where(c =>
                    SqlFunctions.DateDiff("ss", c.LastActivity, DateTimeOffset.UtcNow) >= _zombieThreshold);

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