using System.Diagnostics.CodeAnalysis;
using System.Web.Razor;
using System.Web.WebPages.Razor;

namespace OpenRasta.Codecs.Razor
{
    public class OpenRastaRazorHostFactory
    {
        public static OpenRastaRazorHost CreateHost(RazorCodeLanguage codeLanguage)
        {
            return new OpenRastaRazorHost(codeLanguage);
        }
    }
}