using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web;

namespace OpenRasta.Codecs.Razor
{
    public abstract class RazorViewBase
    {
        public IList<Error> Errors { get; set; }
        public TextWriter Output { get; set; }

        /// <summary>
        ///     Overridden in generated view class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void Execute();

        public void Write(HelperResult result)
        {
            WriteTo(Output, result);
        }

        public void Write(object value)
        {
            WriteTo(Output, value);
        }

        public void WriteLiteral(object value)
        {
            Output.Write(value);
        }

        // This method is called by generated code and needs to stay in sync with the parser
        public void WriteAttribute(string name,
                                   Tuple<string, int> leader,
                                   Tuple<string, int> trailer,
                                   params AttributeValue[] parts)
        {
            WriteLiteral(leader);
            foreach (var part in parts)
            {
                WriteLiteral(part.Prefix.Item1);
                if (part.Literal) WriteLiteral(part.Value.Item1);
                else Write(part.Value.Item1);
            }
            WriteLiteral(trailer);
        }

        // This method is called by generated code and needs to stay in sync with the parser
        public static void WriteTo(TextWriter writer, HelperResult content)
        {
            if (content != null)
            {
                content.WriteTo(writer);
            }
        }

        // This method is called by generated code and needs to stay in sync with the parser
        public static void WriteTo(TextWriter writer, object content)
        {
            writer.Write(HttpUtility.HtmlEncode(content));
        }

        // This method is called by generated code and needs to stay in sync with the parser
        public static void WriteLiteralTo(TextWriter writer, object content)
        {
            writer.Write(content);
        }

        public abstract void SetResource(object resource);
    }

    public abstract class RazorViewBase<T> : RazorViewBase
    {
        public T Resource { get; private set; }

        public override sealed void SetResource(object resource)
        {
            Resource = (T)resource;
        }
    }
}