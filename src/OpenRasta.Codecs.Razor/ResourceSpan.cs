using System;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;

namespace OpenRasta.Codecs.Razor
{
    public class ResourceSpan : CodeSpan
    {
        public ResourceSpan(SourceLocation start, string content, string modelTypeName)
            : base(start, content)
        {
            ResourceTypeName = modelTypeName;
        }

        public string ResourceTypeName
        {
            get;
            private set;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (ResourceTypeName ?? String.Empty).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var span = obj as ResourceSpan;
            return span != null && Equals(span);
        }

        private bool Equals(ResourceSpan span)
        {
            return base.Equals(span) && String.Equals(ResourceTypeName, span.ResourceTypeName, StringComparison.Ordinal);
        }

        public new static ResourceSpan Create(ParserContext context, string modelTypeName)
        {
            return new ResourceSpan(context.CurrentSpanStart, context.ContentBuffer.ToString(), modelTypeName);
        }
    }
}