using System;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public interface IRazorTemplate : ITemplateFile
    {
        Guid GeneratedViewId { get; }
    }

    public class RazorTemplate : Template, IRazorTemplate
    {
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

    }
}