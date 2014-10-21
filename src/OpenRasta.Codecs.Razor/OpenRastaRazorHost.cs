using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;
using System.Web.WebPages;

namespace OpenRasta.Codecs.Razor
{
    public class OpenRastaRazorHost : RazorEngineHost
    {
        internal const string ApplicationInstancePropertyName = "ApplicationInstance";
        internal const string ContextPropertyName = "Context";
        internal const string DefineSectionMethodName = "DefineSection";
        internal const string WebDefaultNamespace = "ASP";
        internal const string WriteToMethodName = "WriteTo";
        internal const string WriteLiteralToMethodName = "WriteLiteralTo";

        internal static readonly string TemplateTypeName = typeof(HelperResult).FullName;

        private static readonly ConcurrentDictionary<string, object> _importedNamespaces = new ConcurrentDictionary<string, object>();
        
        public bool DefaultDebugCompilation { get; set; }

        public OpenRastaRazorHost(RazorCodeLanguage codeLanguage)
        {
            NamespaceImports.Add("System");
            NamespaceImports.Add("System.Collections.Generic");
            NamespaceImports.Add("System.IO");
            NamespaceImports.Add("System.Linq");
            NamespaceImports.Add("System.Net");
            NamespaceImports.Add("System.Web");
            //NamespaceImports.Add("System.Web.Helpers");
            NamespaceImports.Add("System.Web.Security");
            NamespaceImports.Add("System.Web.UI");
            NamespaceImports.Add("System.Web.WebPages");
            
            DefaultNamespace = WebDefaultNamespace;
            GeneratedClassContext = new GeneratedClassContext(GeneratedClassContext.DefaultExecuteMethodName,
                                                              GeneratedClassContext.DefaultWriteMethodName,
                                                              GeneratedClassContext.DefaultWriteLiteralMethodName,
                                                              WriteToMethodName,
                                                              WriteLiteralToMethodName,
                                                              TemplateTypeName,
                                                              DefineSectionMethodName);
            DefaultBaseClass = typeof (RazorViewBase<>).AssemblyQualifiedName;
            DefaultDebugCompilation = true;
            CodeLanguage = codeLanguage;
        }               

        public static void AddGlobalImport(string ns)
        {
            if (String.IsNullOrEmpty(ns)) { throw new ArgumentException("Argument cannot be null or an empty string.", "ns"); }

            _importedNamespaces.TryAdd(ns, null);
        }

        public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
        {
            if (incomingCodeGenerator is CSharpRazorCodeGenerator)
            {
                return new OpenRastaCSharpRazorCodeGenerator(incomingCodeGenerator.ClassName,
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

        public override MarkupParser CreateMarkupParser()
        {
            return new HtmlMarkupParser();
        }        

        public static IEnumerable<string> GetGlobalImports()
        {
            return _importedNamespaces.ToArray().Select(pair => pair.Key);
        }               

        public override void PostProcessGeneratedCode(CodeCompileUnit codeCompileUnit,
                                                      CodeNamespace generatedNamespace,
                                                      CodeTypeDeclaration generatedClass,
                                                      CodeMemberMethod executeMethod)
        {
            base.PostProcessGeneratedCode(codeCompileUnit, generatedNamespace, generatedClass, executeMethod);
            generatedNamespace.Imports.AddRange(GetGlobalImports().Select(s => new CodeNamespaceImport(s)).ToArray());            
        }        
    }
}