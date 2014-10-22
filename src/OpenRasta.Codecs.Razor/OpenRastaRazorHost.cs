using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;

namespace OpenRasta.Codecs.Razor
{
    public sealed class OpenRastaRazorHost : RazorEngineHost
    {
        internal const string PageClassNamePrefix = "_Page_";
        internal const string DefineSectionMethodName = "DefineSection";
        internal const string WebDefaultNamespace = "ASP";
        internal const string WriteToMethodName = "WriteTo";
        internal const string WriteLiteralToMethodName = "WriteLiteralTo";
        internal const string ResolveUrlMethodName = "Href";

        internal static readonly string PageBaseClass = typeof(RazorViewBase).FullName;
        internal static readonly string TemplateTypeName = typeof(HelperResult).FullName;

        private static readonly ConcurrentDictionary<string, object> _importedNamespaces = new ConcurrentDictionary<string, object>();

        private OpenRastaRazorHost()
        {
            if (NamespaceImports != null)
            {
                NamespaceImports.Add("System");
                NamespaceImports.Add("System.Collections.Generic");
                NamespaceImports.Add("System.IO");
                NamespaceImports.Add("System.Linq");
                NamespaceImports.Add("System.Net");
                NamespaceImports.Add("System.Web");
                NamespaceImports.Add("System.Web.Security");
                NamespaceImports.Add("System.Web.UI");
            }

            DefaultNamespace = WebDefaultNamespace;
            GeneratedClassContext = new GeneratedClassContext(GeneratedClassContext.DefaultExecuteMethodName,
                                                              GeneratedClassContext.DefaultWriteMethodName,
                                                              GeneratedClassContext.DefaultWriteLiteralMethodName,
                                                              WriteToMethodName,
                                                              WriteLiteralToMethodName,
                                                              TemplateTypeName,
                                                              DefineSectionMethodName)
            {
                ResolveUrlMethodName = ResolveUrlMethodName
            };

            DefaultBaseClass = PageBaseClass;
            DefaultDebugCompilation = true;
            EnableInstrumentation = false;
        }

        public OpenRastaRazorHost(RazorCodeLanguage codeLanguage) : this()
        {
            CodeLanguage = codeLanguage;
            //DefaultClassName = GetClassName(VirtualPath);
            //EnableInstrumentation = new InstrumentationService().IsAvailable;
        }


        public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
        {
            if (incomingCodeGenerator is CSharpRazorCodeGenerator)
            {
                return new OpenRastaCSharpRazorCodeGenerator(
                    incomingCodeGenerator.ClassName,
                    incomingCodeGenerator.RootNamespaceName,
                    incomingCodeGenerator.SourceFileName,
                    incomingCodeGenerator.Host);
            }
            if (incomingCodeGenerator is VBRazorCodeGenerator)
            {
                throw new InvalidOperationException("VB not supported yet.");
            }
            return base.DecorateCodeGenerator(incomingCodeGenerator);
        }

        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            if (incomingCodeParser is CSharpCodeParser)
            {
                return new OpenRastaCSharpRazorCodeParser();
            }
            if (incomingCodeParser is VBCodeParser)
            {
                throw new InvalidOperationException("VB not supported yet.");
            }
            return base.DecorateCodeParser(incomingCodeParser);
        }

        public bool DefaultDebugCompilation { get; set; }

        public static void AddGlobalImport(string ns)
        {
            if (String.IsNullOrEmpty(ns))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Argument {0} cannot be null or an empty string.", "ns"), "ns");
            }

            _importedNamespaces.TryAdd(ns, null);
        }

        public override ParserBase CreateMarkupParser()
        {
            return new HtmlMarkupParser();
        }

        public static IEnumerable<string> GetGlobalImports()
        {
            return _importedNamespaces.ToArray().Select(pair => pair.Key);
        }

        public override void PostProcessGeneratedCode(CodeGeneratorContext context)
        {
            base.PostProcessGeneratedCode(context);

            context.Namespace.Imports.AddRange(GetGlobalImports().Select(s => new CodeNamespaceImport(s)).ToArray());
        }
    }
}