using System.Reflection;

using OpenRasta.Configuration.Fluent;
using OpenRasta.DI;

namespace OpenRasta.Codecs.Razor
{
    public static class RazorConfigurationExtensions
    {
        /// <summary>
        ///     Adds an html rendering of a resource using a Razor view.
        /// </summary>
        public static ICodecDefinition RenderedByRazor(this ICodecParentDefinition codecParentDefinition, string pageVirtualPath)
        {
            return codecParentDefinition.TranscodedBy<RazorCodec>(new { index = pageVirtualPath });
        }

        /// <summary>
        ///     Adds an html rendering of a resource using a Razor view.
        /// </summary>
        public static ICodecDefinition RenderedByRazor(this ICodecParentDefinition codecParentDefinition, object viewVirtualPaths)
        {
            return codecParentDefinition.TranscodedBy<RazorCodec>(viewVirtualPaths);
        }

        /// <summary>
        ///     Registers embedded view provider.
        /// </summary>
        /// <param name="uses"></param>
        /// <param name="assembly">Assembly containing views.</param>
        /// <param name="baseNamespace">Base namespace containing views.</param>
        public static void ViewsEmbeddedInTheAssembly(this IUses uses, Assembly assembly, string baseNamespace)
        {
            uses.Resolver.AddDependencyInstance(typeof(IViewProvider), new EmbeddedViewProvider(assembly, baseNamespace));
        }
    }
}