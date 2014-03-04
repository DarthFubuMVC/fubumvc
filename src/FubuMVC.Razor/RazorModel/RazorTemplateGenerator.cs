using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Razor;
using System.Web.Razor.Parser.SyntaxTree;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public interface IRazorTemplateGenerator
    {
        GeneratorResults GenerateCode(ViewDescriptor<IRazorTemplate> descriptor, string className, RazorEngineHost host);
    }

    public class RazorTemplateGenerator : IRazorTemplateGenerator
    {
         public GeneratorResults GenerateCode(ViewDescriptor<IRazorTemplate> descriptor, string className, RazorEngineHost host)
         {
             var engine = new RazorTemplateEngine(host);
             GeneratorResults results;
             using (var fileStream = new FileStream(descriptor.Template.FilePath, FileMode.Open, FileAccess.Read))
             using (var reader = new StreamReader(fileStream))
             {
                 results = engine.GenerateCode(reader, className, host.DefaultNamespace, descriptor.ViewPath);
             }

             if (!results.Success)
             {
                 throw CreateExceptionFromParserError(results.ParserErrors.Last(), descriptor.Name());
             }
             return results;
         }

         private static HttpParseException CreateExceptionFromParserError(RazorError error, string virtualPath)
         {
             return new HttpParseException(error.Message + Environment.NewLine, null, virtualPath, null, error.Location.LineIndex + 1);
         }
    }
}