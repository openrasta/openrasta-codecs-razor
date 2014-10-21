using OpenRasta.Codecs.Razor.TestApp.Resources;

namespace OpenRasta.Codecs.Razor.TestApp.Handlers
{
    public class TestHandler
    {
        public TestResource Get()
        {
            return new TestResource
                       {
                           TestString = "Hello, OpenRasta!"
                       };
        }
    }
}