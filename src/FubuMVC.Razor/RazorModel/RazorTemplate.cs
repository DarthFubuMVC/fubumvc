using System;
using System.Linq;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.Registration;

namespace FubuMVC.Razor.RazorModel
{
    public interface IRazorTemplate : ITemplateFile
    {
        Guid GeneratedViewId { get; }
    }

    public class RazorTemplate : Template, IRazorTemplate
    {
        private static readonly IViewParser ViewParser = new ViewParser();
        private readonly Guid _generatedViewId = Guid.NewGuid();

        public RazorTemplate(IFubuFile file) : base(file)
        {
        }

        public RazorTemplate(string filePath, string rootPath, string origin) : base(filePath, rootPath, origin)
        {
        }

        public Guid GeneratedViewId
        {
            get { return _generatedViewId; }
        }


        // TODO -- some commonality here between RazorTemplate and SparkTemplate!
        protected override Parsing createParsing()
        {
            var chunks = ViewParser.Parse(FilePath).ToList();

            return new Parsing
            {
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel(),
                Namespaces = chunks.Namespaces()
            };
        }
    }
}