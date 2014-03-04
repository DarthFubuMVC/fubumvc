using System.Collections.Generic;
using System.IO;
using System.Web.Razor;
using System.Web.Razor.Parser.SyntaxTree;
using FubuMVC.Razor.Core;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.Registration
{
    public class ViewParser : IViewParser
    {
        public IEnumerable<Span> Parse(string viewFile)
        {
            var codeLanguage = RazorCodeLanguageFactory.Create(viewFile.FileExtension());

            using (var fileStream = new FileStream(viewFile, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream))
            {
                var templateEngine = new RazorTemplateEngine(new RazorEngineHost(codeLanguage));
                var parseResults = templateEngine.ParseTemplate(reader);
                return parseResults.Document.Flatten();
            }
        }
    }

    public interface IViewParser
    {
        IEnumerable<Span> Parse(string viewFile);
    }
}