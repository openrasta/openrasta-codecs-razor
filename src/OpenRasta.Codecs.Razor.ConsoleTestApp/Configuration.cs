using System.Reflection;

using OpenRasta.Codecs.Razor.ConsoleTestApp.Handlers;
using OpenRasta.Codecs.Razor.ConsoleTestApp.Resources;
using OpenRasta.Configuration;

namespace OpenRasta.Codecs.Razor.ConsoleTestApp
{
    public class Configuration : IConfigurationSource
    {
        public void Configure()
        {
            using (OpenRastaConfiguration.Manual)
            {
                ResourceSpace.Uses.ViewsEmbeddedInTheAssembly(Assembly.GetExecutingAssembly(), "OpenRasta.Codecs.Razor.ConsoleTestApp.Views");

                ResourceSpace.Has.ResourcesOfType<TestResource>().AtUri("/home").And.AtUri("/").HandledBy<TestHandler>().RenderedByRazor(new { index = "TestView.cshtml" });
            }
        }
    }
}