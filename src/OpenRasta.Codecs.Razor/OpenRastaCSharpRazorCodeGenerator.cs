using System.CodeDom;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser.SyntaxTree;

namespace OpenRasta.Codecs.Razor
{
    public class OpenRastaCSharpRazorCodeGenerator : CSharpRazorCodeGenerator
    {
        public OpenRastaCSharpRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host)
        {
        }

        protected override bool TryVisitSpecialSpan(Span span)
        {
            return TryVisit<ResourceSpan>(span, VisitResourceSpan);
        }

        private void VisitResourceSpan(ResourceSpan span)
        {
            string modelName = span.ResourceTypeName;
            var baseType = new CodeTypeReference(Host.DefaultBaseClass, new CodeTypeReference(modelName));

            GeneratedClass.BaseTypes.Clear();
            GeneratedClass.BaseTypes.Add(baseType);

            if (DesignTimeMode)
            {
                WriteHelperVariable(span.Content, "__modelHelper");
            }
        }
    }
}