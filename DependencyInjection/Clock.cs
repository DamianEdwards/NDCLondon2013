using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DependencyInjection
{
    public class Clock : Hub
    {
        private readonly IClockService _clock;

        public Clock(IClockService clock)
        {
            _clock = clock;
        }

        public void Start()
        {
            _clock.Start();
        }

        public void Stop()
        {
            _clock.Stop();
        }
    }
}