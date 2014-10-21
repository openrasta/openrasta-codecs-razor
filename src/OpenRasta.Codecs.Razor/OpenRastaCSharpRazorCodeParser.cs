using System;
using System.Globalization;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;

namespace OpenRasta.Codecs.Razor
{
    public class OpenRastaCSharpRazorCodeParser : CSharpCodeParser
    {
        private const string ResourceKeyword = "resource";
        private SourceLocation? _endInheritsLocation;
        private bool _modelStatementFound;

        public OpenRastaCSharpRazorCodeParser()
        {
            RazorKeywords.Add(ResourceKeyword, WrapSimpleBlockParser(BlockType.Directive, ParseResourceStatement));
        }

        protected override bool ParseInheritsStatement(CodeBlockInfo block)
        {
            _endInheritsLocation = CurrentLocation;
            bool result = base.ParseInheritsStatement(block);
            CheckForInheritsAndResourceStatements();
            return result;
        }

        private void CheckForInheritsAndResourceStatements()
        {
            if (_modelStatementFound && _endInheritsLocation.HasValue)
            {
                OnError(_endInheritsLocation.Value, String.Format(CultureInfo.CurrentCulture, "The 'inherits' keyword is not allowed when a '{0}' keyword is used.", ResourceKeyword));
            }
        }

        private bool ParseResourceStatement(CodeBlockInfo block)
        {
            End(MetaCodeSpan.Create);

            SourceLocation endModelLocation = CurrentLocation;
            if (_modelStatementFound)
            {
                OnError(endModelLocation, String.Format(CultureInfo.CurrentCulture, "Only one '{0}' statement is allowed in a file.", ResourceKeyword));
            }

            _modelStatementFound = true;

            // Accept Whitespace up to the new line or non-whitespace character
            Context.AcceptWhiteSpace(false);

            string typeName = null;
            if (ParserHelpers.IsIdentifierStart(CurrentCharacter))
            {
                using (Context.StartTemporaryBuffer())
                {
                    // Accept a dotted-identifier, but allow <>
                    AcceptTypeName();
                    typeName = Context.ContentBuffer.ToString();
                    Context.AcceptTemporaryBuffer();
                }
            }
            else
            {
                OnError(endModelLocation, String.Format(CultureInfo.CurrentCulture, "The '{0}' keyword must be followed by a type name on the same line.", ResourceKeyword));
            }
            CheckForInheritsAndResourceStatements();
            End(ResourceSpan.Create(Context, typeName));
            return false;
        }
    }
}