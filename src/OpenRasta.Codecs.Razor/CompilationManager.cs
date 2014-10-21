using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using OpenRasta.Web;

namespace OpenRasta.Codecs.Razor
{
    public class CompilationManager
    {
        private static readonly object _lock = new object();
        private static Dictionary<string, Type> _compiledTypes = new Dictionary<string, Type>();
        private static readonly CodeDomProvider _compiler = CreateCompiler();

        public static Type GetCompiledType(string key, Func<CompilationData> compilationDataGenerator)
        {
            Type existing;
            if (_compiledTypes.TryGetValue(key, out existing))
            {
                return existing;
            }
            lock (_lock)
            {
                Type newlyCompiledType = CompileType(compilationDataGenerator);
                var newCompiledTypes = new Dictionary<string, Type>(_compiledTypes);
                newCompiledTypes[key] = newlyCompiledType;
                _compiledTypes = newCompiledTypes;
                return newlyCompiledType;
            }
        }

        private static Type CompileType(Func<CompilationData> compilationDataGenerator)
        {
            var compilationData = compilationDataGenerator();
            var code = compilationData.Code;
            var compiled = _compiler.CompileAssemblyFromDom(CreateCompilerParameters(compilationData.AdditionalAssemblies), code);

            if (compiled.Errors.HasErrors)
            {
                var sourceCode = new StringBuilder();                
                _compiler.GenerateCodeFromCompileUnit(code, new StringWriter(sourceCode), new CodeGeneratorOptions());
                throw new HttpCompileException(compiled, sourceCode.ToString());
            }
            
            return compiled.CompiledAssembly.GetTypes().First();
        }

        private static CompilerParameters CreateCompilerParameters(IEnumerable<string> additionalAssemblies)
        {
            var referencedAssemblies = new List<string>
                                           {
                                               "System.dll",
                                               "System.Core.dll",
                                               "System.Web.dll",
                                               "System.Data.dll",
                                               "System.Web.Extensions.dll",                                               
                                               "Microsoft.CSharp.dll",                                                                                              
                                               typeof(IRequest).Assembly.Location,
                                               typeof(System.Web.WebPages.HelperResult).Assembly.Location,
                                               typeof(StandAloneBuildManager).Assembly.Location
                                           };            

            var parameters = new CompilerParameters();

            foreach (var current in additionalAssemblies)
            {
                if (!referencedAssemblies.Any(x => current.EndsWith("\\" + x) || current.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    referencedAssemblies.Add(current);
                }
            }

            parameters.ReferencedAssemblies.AddRange(referencedAssemblies.ToArray());
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;

            return parameters;
        }

        private static CodeDomProvider CreateCompiler()
        {
            var options = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
            var compiler = CodeDomProvider.CreateProvider("C#", options);
            return compiler;
        }
    }
}