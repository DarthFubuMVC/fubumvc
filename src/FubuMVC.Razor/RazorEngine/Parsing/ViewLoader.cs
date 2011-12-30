using System.Collections.Generic;
using System.IO;
using System.Web.Razor;
using System.Web.Razor.Parser.SyntaxTree;
using FubuMVC.Razor.FileSystem;
using CSharpRazorCodeLanguage = RazorEngine.Compilation.CSharp.CSharpRazorCodeLanguage;

namespace FubuMVC.Razor.RazorEngine.Parsing
{
    public class ViewLoader : IViewLoader
    {
        private IViewFile _viewFile;
        private long _lastModified;

        public IViewFolder ViewFolder { get; set; }

        public IEnumerable<Span> Load(string path)
        {
            _viewFile = ViewFolder.GetViewSource(path);
            _lastModified = _viewFile.LastModified;
            using(var fileStream = _viewFile.OpenViewStream())
            using(var reader = new StreamReader(fileStream))
            {
                var engine = new RazorTemplateEngine(new RazorEngineHost(new CSharpRazorCodeLanguage(true)));
                var parseResults = engine.ParseTemplate(reader);
                return parseResults.Document.Flatten();
            }
        }

        public bool IsCurrent()
        {
            return _viewFile.LastModified == _lastModified;
        }
    }

    public interface IViewLoader
    {
        IEnumerable<Span> Load(string path);
        bool IsCurrent();
    }
}