using System;

namespace OpenRasta.Codecs.Razor.ConsoleTestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var h = new HttpListenerHostWithConfiguration { Configuration = new Configuration() };
            h.Initialize(new[] { "http://+:9222/" }, "/", null);
            h.StartListening();


            h.IncomingRequestReceived += h_IncomingRequestReceived;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        static void h_IncomingRequestReceived(object sender, Hosting.IncomingRequestReceivedEventArgs e)
        {
            Console.WriteLine("Incoming request: " + e.Context.Request.Uri);
        }
    }
}