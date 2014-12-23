using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
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

        public void WriteAttribute(string attr,
                                           Tuple<string, int> token1,
                                           Tuple<string, int> token2,
                                           Tuple<Tuple<string, int>, Tuple<object, int>, bool> token3)
        {
            var stringBuilder = new StringBuilder();
            if (token1 != null) stringBuilder.Append(token1.Item1);
            if (token2 != null) stringBuilder.Append(token3.Item2.Item1);
            if (token3 != null) stringBuilder.Append(token2.Item1);

            Output.Write(stringBuilder.ToString());
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