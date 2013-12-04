using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace SignalRAndNancy
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start("http://localhost:1337"))
            {
                Console.WriteLine("Hosting server on http://localhost:1337");
                Console.ReadKey();
            }
        }
    }
}
