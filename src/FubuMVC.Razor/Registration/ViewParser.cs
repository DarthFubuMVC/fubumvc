using System.Collections.Generic;
using System.IO;
using System.Web.Razor;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using FubuMVC.Razor.FileSystem;
using CSharpRazorCodeLanguage = RazorEngine.Compilation.CSharp.CSharpRazorCodeLanguage;

namespace FubuMVC.Razor.Registration
{
    public class ViewParser : IViewParser
    {
        private readonly IViewFile _viewFile;
        private long _lastModified;

        public ViewParser(IViewFile viewFile)
        {
            _viewFile = viewFile;
        }

        public IEnumerable<Span> Parse()
        {
            _lastModified = _viewFile.LastModified;

            using(var fileStream = _viewFile.OpenViewStream())
            using (var reader = new StreamReader(fileStream))
            {
                var engine = new RazorTemplateEngine(
                        new global::RazorEngine.Compilation.RazorEngineHost(new CSharpRazorCodeLanguage(false),
                                                                            () => new HtmlMarkupParser()));
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