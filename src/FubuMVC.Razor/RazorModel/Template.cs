using System;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public interface IRazorTemplate : ITemplateFile
    {
        bool IsCurrent();
        Guid GeneratedViewId { get; }
		
        string ViewPath { get; set; }
        IRazorDescriptor Descriptor { get; set; }
    }

    public class Template : IRazorTemplate
    {
        public Template(string filePath, string rootPath, string origin)
        {
            FilePath = filePath;
            RootPath = rootPath;
            Origin = origin;
            GeneratedViewId = Guid.NewGuid();
            Descriptor = new NulloDescriptor();
        }

        public string FilePath { get; private set; }
        public string RootPath { get; private set; }
        public string Origin { get; private set; }
        public Guid GeneratedViewId { get; private set; }
		
        public string ViewPath { get; set; }
        public IRazorDescriptor Descriptor { get; set; }

        public bool IsCurrent()
        {
            return Descriptor.IsCurrent();
        }

	    public override string ToString()
        {
            return FilePath;
        }

        public bool Equals(Template other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.FilePath, FilePath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Template)) return false;
            return Equals((Template) obj);
        }

        public override int GetHashCode()
        {
            return (FilePath != null ? FilePath.GetHashCode() : 0);
        }
    }
}