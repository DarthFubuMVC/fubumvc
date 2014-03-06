using System;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public interface IRazorTemplate : ITemplateFile
    {
        Guid GeneratedViewId { get; }
    }

    public class RazorTemplate : IRazorTemplate
    {
        public RazorTemplate(string filePath, string root, string origin) : this()
        {
            FilePath = filePath;
            RootPath = root;
            Origin = origin;
        }

        public RazorTemplate()
        {
            GeneratedViewId = Guid.NewGuid();
            Descriptor = new NulloDescriptor();
        }

        public string FilePath { get; set; }
        public string RootPath { get; set; }
        public string Origin { get; set; }
        public Guid GeneratedViewId { get; private set; }
		
        public string ViewPath { get; set; }
        public ITemplateDescriptor Descriptor { get; set; }

	    public override string ToString()
        {
            return FilePath;
        }
    }
}