using System.CodeDom;
using System.Web.Razor;
using System.Web.Razor.Generator;

namespace OpenRasta.Codecs.Razor
{
    public class OpenRastaCSharpRazorCodeGenerator : CSharpRazorCodeGenerator
    {
        private const string DefaultResourceTypeName = "dynamic";

        public OpenRastaCSharpRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host)
        {
            var webPageRazorHost = host as OpenRastaRazorHost;

            if (webPageRazorHost == null) return;

            SetBaseType(DefaultResourceTypeName);
        }

        private void SetBaseType(string resourceTypeName)
        {
            var codeTypeReference = new CodeTypeReference(Context.Host.DefaultBaseClass + "<" + resourceTypeName + ">");
            Context.GeneratedClass.BaseTypes.Clear();
            Context.GeneratedClass.BaseTypes.Add(codeTypeReference);
        }
    }
}
