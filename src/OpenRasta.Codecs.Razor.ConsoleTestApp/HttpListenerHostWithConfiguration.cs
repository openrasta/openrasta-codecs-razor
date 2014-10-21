using OpenRasta.Configuration;
using OpenRasta.DI;
using OpenRasta.Hosting.HttpListener;

namespace OpenRasta.Codecs.Razor.ConsoleTestApp
{
    public class HttpListenerHostWithConfiguration : HttpListenerHost
    {
        public IConfigurationSource Configuration { get; set; }

        public override bool ConfigureRootDependencies(IDependencyResolver resolver)
        {
            var result = base.ConfigureRootDependencies(resolver);
            if (result && Configuration != null)
            {
                resolver.AddDependencyInstance<IConfigurationSource>(Configuration);
            }
            return result;
        }
    }
}