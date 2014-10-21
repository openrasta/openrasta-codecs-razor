using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;

namespace OpenRasta.Codecs.Razor
{
    [BuildProviderAppliesTo(BuildProviderAppliesTo.Web | BuildProviderAppliesTo.Code)]
    public class OpenRastaRazorBuildProvider : BuildProvider
    {
        private static bool? _isFullTrust;
        private CodeCompileUnit _generatedCode;
        private OpenRastaRazorHost _host;
        private string _physicalPath;

        private OpenRastaRazorHost Host
        {
            get
            {
                if (_host == null)
                {
                    _host = CreateHost();
                }
                return _host;
            }
        }

        public string PhysicalPath
        {
            get
            {
                MapPhysicalPath();
                return _physicalPath;
            }
            set { _physicalPath = value; }
        }

        private void MapPhysicalPath()
        {
            if (_physicalPath == null && HostingEnvironment.IsHosted)
            {
                string path = HostingEnvironment.MapPath(VirtualPath);
                if (!String.IsNullOrEmpty(path) && File.Exists(path))
                {
                    _physicalPath = path;
                }
            }
        }

        private CodeCompileUnit GeneratedCode
        {
            get
            {
                EnsureGeneratedCode();
                return _generatedCode;
            }
        }

        public override CompilerType CodeCompilerType
        {
            get
            {
                EnsureGeneratedCode();
                CompilerType compilerType = GetDefaultCompilerTypeForLanguage(Host.CodeLanguage.LanguageName);
                if (_isFullTrust != false && Host.DefaultDebugCompilation)
                {
                    try
                    {
                        SetIncludeDebugInfoFlag(compilerType);
                        _isFullTrust = true;
                    }
                    catch (SecurityException)
                    {
                        _isFullTrust = false;
                    }
                }
                return compilerType;
            }
        }        

        public override Type GetGeneratedType(CompilerResults results)
        {
            return results.CompiledAssembly.GetType(String.Format(CultureInfo.CurrentCulture, "{0}.{1}", Host.DefaultNamespace, GetClassName()));
        }

        public override void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
            assemblyBuilder.AddCodeCompileUnit(this, GeneratedCode);
        }

        private OpenRastaRazorHost CreateHost()
        {
            return OpenRastaRazorHostFactory.CreateHost(GetCodeLanguage());
        }        

        private RazorCodeLanguage GetCodeLanguage()
        {
            RazorCodeLanguage language = DetermineCodeLanguage(VirtualPath);
            if (language == null && !String.IsNullOrEmpty(PhysicalPath))
            {
                language = DetermineCodeLanguage(PhysicalPath);
            }

            if (language == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "Could not determine the code language for '{0}'", VirtualPath));
            }

            return language;
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

        private void EnsureGeneratedCode()
        {
            if (_generatedCode == null)
            {
                var engine = new RazorTemplateEngine(Host);
                GeneratorResults results;
                using (TextReader reader = OpenReader())
                {
                    results = engine.GenerateCode(reader, GetClassName(), Host.DefaultNamespace, null);
                }
                if (!results.Success)
                {
                    throw CreateExceptionFromParserError(results.ParserErrors.Last(), VirtualPath);
                }
                _generatedCode = results.GeneratedCode;
            }
        }

        private string GetClassName()
        {
            return ParserHelpers.SanitizeClassName(Path.GetFileName(VirtualPath));
        }

        private static HttpParseException CreateExceptionFromParserError(RazorError error, string virtualPath)
        {
            return new HttpParseException(error.Message + Environment.NewLine, null, virtualPath, null, error.Location.LineIndex + 1);
        }

        private static void SetIncludeDebugInfoFlag(CompilerType compilerType)
        {
            compilerType.CompilerParameters.IncludeDebugInformation = true;
        }
    }
}