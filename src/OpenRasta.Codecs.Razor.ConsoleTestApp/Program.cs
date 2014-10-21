using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRasta.Codecs.Razor.ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var h = new HttpListenerHostWithConfiguration { Configuration = new Configuration() };
            h.Initialize(new[] { "http://+:9222/" }, "/", null);
            h.StartListening();
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }
}
