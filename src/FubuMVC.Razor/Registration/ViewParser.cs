using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Razor;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using FubuMVC.Razor.FileSystem;
using CSharpRazorCodeLanguage = RazorEngine.Compilation.CSharp.CSharpRazorCodeLanguage;
using VBRazorCodeLanguage = RazorEngine.Compilation.VisualBasic.VBRazorCodeLanguage;

namespace FubuMVC.Razor.Registration
{
    public class ViewParser : IViewParser
    {
        private readonly IViewFile _viewFile;

        public ViewParser(IViewFile viewFile)
        {
            _viewFile = viewFile;
        }

        public IEnumerable<Span> Parse()
        {
            RazorCodeLanguage language;
            switch (_viewFile.Extension)
            {
                case ".cshtml":
                    language = new CSharpRazorCodeLanguage(true);
                    break;
                case ".vbhtml":
                    language = new VBRazorCodeLanguage(true);
                    break;
                default:
                    throw new ArgumentException("Invalid extension for Razor engine.");
            }

            using(var fileStream = _viewFile.OpenViewStream())
            using (var reader = new StreamReader(fileStream))
            {
                var engine = new RazorTemplateEngine(new RazorEngine.Compilation.RazorEngineHost(language, () => new HtmlMarkupParser()));
                var parseResults = engine.ParseTemplate(reader);
                return parseResults.Document.Flatten();
            }
        }
    }

    public interface IViewParser
    {
        IEnumerable<Span> Parse();
    }
}