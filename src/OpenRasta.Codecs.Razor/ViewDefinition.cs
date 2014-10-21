using System.IO;
using System.Reflection;

namespace OpenRasta.Codecs.Razor
{
    public class ViewDefinition
    {
        private readonly TextReader _contents;
        private readonly string _fileName;
        private readonly Assembly _viewAssembly;

        public ViewDefinition(string fileName, TextReader contents, Assembly viewAssembly)
        {
            _fileName = fileName;
            _viewAssembly = viewAssembly;
            _contents = contents;
        }

        public Assembly ViewAssembly
        {
            get { return _viewAssembly; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public TextReader Contents
        {
            get { return _contents; }
        }
    }
}