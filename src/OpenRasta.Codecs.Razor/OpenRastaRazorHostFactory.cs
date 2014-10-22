using System.Web.Razor;

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