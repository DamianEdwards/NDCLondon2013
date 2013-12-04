using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependencyInjection
{
    public interface IClockService
    {
        void Stop();
        void Start();
    }
}