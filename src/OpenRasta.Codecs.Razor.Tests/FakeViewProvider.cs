using System.IO;
using System.Reflection;

namespace OpenRasta.Codecs.Razor.Tests
{
    public class FakeViewProvider : IViewProvider
    {
        private readonly string _viewCode;

        public FakeViewProvider(string viewCode)
        {
            _viewCode = viewCode;
        }

        public ViewDefinition GetViewDefinition(string path)
        {
            return new ViewDefinition("ViewFile.cshtml", new StringReader(_viewCode), Assembly.GetExecutingAssembly());
        }
    }
}