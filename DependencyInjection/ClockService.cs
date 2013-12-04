using System;
using System.Threading;
using Microsoft.AspNet.SignalR;

namespace DependencyInjection
{
    public class ClockService : IClockService
    {
        private Timer _timer;
        private readonly IHubContext _context;

        public ClockService(IHubContext context)
        {
            _context = context;
        }

        public void Start()
        {
            if (_timer != null)
            {
                return;
            }

            _timer = new Timer(Fire, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private void Fire(object sender)
        {
            _context.Clients.All.tick(DateTimeOffset.UtcNow);
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}