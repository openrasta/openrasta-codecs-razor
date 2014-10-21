using OpenRasta.Codecs.Razor.TestApp.Handlers;
using OpenRasta.Codecs.Razor.TestApp.Resources;
using OpenRasta.Configuration;

namespace OpenRasta.Codecs.Razor.TestApp
{
    public class Configuration : IConfigurationSource //, IDependencyResolverFactory
    {
        public void Configure()
        {
            using (OpenRastaConfiguration.Manual)
            {                
                ResourceSpace.Has.ResourcesOfType<TestResource>()
                    .AtUri("/home")
                    .And.AtUri("/")
                    .HandledBy<TestHandler>()
                    .RenderedByRazor(new {index = "~/Views/TestView.cshtml"});           
            }
        }
    }
}