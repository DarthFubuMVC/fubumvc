using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Razor;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using FubuMVC.Razor.FileSystem;
using RazorEngine.Compilation;
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

            IEnumerable<Span> result = null;

            OpenReader(reader =>
            {
                var engine =
                    new RazorTemplateEngine(
                        new global::RazorEngine.Compilation.RazorEngineHost(new CSharpRazorCodeLanguage(false),
                                                                            () => new HtmlMarkupParser()));
                var parseResults = engine.ParseTemplate(reader);
                result =  parseResults.Document.Flatten();
            });

            return result;
        }

        private void OpenReader(Action<StreamReader> action)
        {
            using(var fileStream = _viewFile.OpenViewStream())
            using(var reader = new StreamReader(fileStream))
            {
                action(reader);
            }
        }

        public bool IsCurrent()
        {
            return _viewFile.LastModified == _lastModified;
        }

        public IViewFile ViewFile { get { return _viewFile; } }
    }

    public interface IViewLoader
    {
        IEnumerable<Span> Load(string path);
        bool IsCurrent();
        IViewFile ViewFile { get; }
    }
}