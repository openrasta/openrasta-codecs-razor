using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenRasta.Codecs.Razor
{
    public class EmbeddedViewProvider : IViewProvider
    {
        private readonly Assembly _assembly;
        private readonly string _baseNamespace;

        public EmbeddedViewProvider(Assembly assembly, string baseNamespace)
        {
            _assembly = assembly;
            _baseNamespace = baseNamespace;
        }

        public ViewDefinition GetViewDefinition(string path)
        {
            path = Regex.Replace(path, "^(~/)|/", ".");

            var stream = _assembly.GetManifestResourceStream(_baseNamespace + path);
            if (stream == null)
            {
                return null;
            }
            var reader = new StreamReader(stream);
            return new ViewDefinition(path, reader,_assembly);
        }
    }
}