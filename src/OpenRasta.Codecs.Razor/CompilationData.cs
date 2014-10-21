using System;
using System.CodeDom;
using System.Collections.Generic;

namespace OpenRasta.Codecs.Razor
{
    public class CompilationData
    {
        private readonly IEnumerable<string> _additionalAssemblies;
        private readonly CodeCompileUnit _code;

        public CompilationData(IEnumerable<string> additionalAssemblies, CodeCompileUnit code)
        {
            _additionalAssemblies = additionalAssemblies;
            _code = code;
        }

        public CodeCompileUnit Code
        {
            get { return _code; }
        }

        public IEnumerable<string> AdditionalAssemblies
        {
            get { return _additionalAssemblies; }
        }
    }
}