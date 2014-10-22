using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Razor;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;

namespace OpenRasta.Codecs.Razor
{
    public class StandAloneBuildManager : IBuildManager
    {
        private readonly IViewProvider _viewProvider;

        public StandAloneBuildManager(IViewProvider viewProvider)
        {
            _viewProvider = viewProvider;
        }

        public Type GetCompiledType(string path)
        {
            return CompilationManager.GetCompiledType(path, () => GenerateCode(path));
        }

        private CompilationData GenerateCode(string path)
        {
            ViewDefinition viewDefinition = _viewProvider.GetViewDefinition(path);
            OpenRastaRazorHost host = OpenRastaRazorHostFactory.CreateHost(DetermineCodeLanguage(path));
            var engine = new RazorTemplateEngine(host);
            GeneratorResults results;
            using (TextReader reader = viewDefinition.Contents)
            {
                results = engine.GenerateCode(reader, GetClassName(viewDefinition.FileName), host.DefaultNamespace, viewDefinition.FileName);
            }
            if (!results.Success)
            {
                throw CreateExceptionFromParserError(results.ParserErrors.Last(), path);
            }
            return new CompilationData(GetReferencedAssemblies(viewDefinition), results.GeneratedCode);
        }

        private static IEnumerable<string> GetReferencedAssemblies(ViewDefinition viewDefinition)
        {
            List<string> referenced = viewDefinition.ViewAssembly.GetReferencedAssemblies().Select(x => Assembly.ReflectionOnlyLoad(x.FullName)).Select(x => x.Location).ToList();
            referenced.Add(viewDefinition.ViewAssembly.Location);
            return referenced;
        }

        private static RazorCodeLanguage DetermineCodeLanguage(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            // Use an if rather than else-if just in case Path.GetExtension returns null for some reason
            if (String.IsNullOrEmpty(extension))
            {
                return null;
            }
            if (extension[0] == '.')
            {
                extension = extension.Substring(1); // Trim off the dot
            }

            // Look up the language
            // At the moment this only deals with code languages: cs, vb, etc., but in theory we could have MarkupLanguageServices which allow for
            // interesting combinations like: vbcss, csxml, etc.
            RazorCodeLanguage language = RazorCodeLanguage.GetLanguageByExtension(extension);
            return language;
        }

        private static string GetClassName(string fileName)
        {
            return ParserHelpers.SanitizeClassName(fileName);
        }

        private static HttpParseException CreateExceptionFromParserError(RazorError error, string virtualPath)
        {
            return new HttpParseException(error.Message + Environment.NewLine, null, virtualPath, null, error.Location.LineIndex + 1);
        }
    }
}