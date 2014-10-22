using OpenRasta.Codecs.Razor.ConsoleTestApp.Resources;

namespace OpenRasta.Codecs.Razor.ConsoleTestApp.Handlers
{
    public class TestHandler
    {
        public TestResource Get()
        {
            return new TestResource { TestString = "Hello, OpenRasta!" };
        }
    }
}