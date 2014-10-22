using System.Globalization;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;
using System.Web.Razor.Text;

namespace OpenRasta.Codecs.Razor
{
    public class OpenRastaCSharpRazorCodeParser : CSharpCodeParser
    {
        private const string ResourceKeyword = "resource";
        private const string GenericTypeFormatString = "{0}<{1}>";
        private SourceLocation? _endInheritsLocation;
        private bool _modelStatementFound;

        public OpenRastaCSharpRazorCodeParser()
        {
            MapDirectives(ResourceDirective, new string[1] { ResourceKeyword });
        }

        protected override void InheritsDirective()
        {
            AcceptAndMoveNext();
            _endInheritsLocation = CurrentLocation;
            InheritsDirectiveCore();
            CheckForInheritsAndModelStatements();
        }

        private void CheckForInheritsAndModelStatements()
        {
            if (!_modelStatementFound || !_endInheritsLocation.HasValue)
            {
                return;
            }
            Context.OnError(_endInheritsLocation.Value, string.Format(CultureInfo.CurrentCulture, "The 'inherits' keyword is not allowed when a '{0}' keyword is used.", new object[1] { ResourceKeyword }));
        }

        protected virtual void ResourceDirective()
        {
            AcceptAndMoveNext();
            SourceLocation currentLocation = CurrentLocation;

            BaseTypeDirective(string.Format(CultureInfo.CurrentCulture, "The '{0}' keyword must be followed by a type name on the same line.", new object[1] { ResourceKeyword }), CreateResourceCodeGenerator);
            
            if (_modelStatementFound)
            {
                Context.OnError(currentLocation, string.Format(CultureInfo.CurrentCulture, "Only one '{0}' statement is allowed in a file.", new object[1] { ResourceKeyword }));
            }

            _modelStatementFound = true;
            CheckForInheritsAndModelStatements();
        }

        private SpanCodeGenerator CreateResourceCodeGenerator(string resource)
        {
            return new SetResourceTypeCodeGenerator(resource, GenericTypeFormatString);
        }
    }
}