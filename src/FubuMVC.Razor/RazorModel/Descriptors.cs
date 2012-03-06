using System;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.FileSystem;

namespace FubuMVC.Razor.RazorModel
{
    public interface IRazorDescriptor
    {
        string Name { get; }
        Type ViewModel { get; set; }
        bool IsCurrent();
        string RelativePath();
        IRazorTemplate Template { get; }
        IRazorTemplate Master { get; }
        IViewFile ViewFile { get; }
    }

    public class ViewDescriptor : IRazorDescriptor
    {
        private readonly IRazorTemplate _template;
        public ViewDescriptor(IRazorTemplate template)
        {
            _template = template;
        }

        string IRazorDescriptor.Name { get { return "View"; } }

        public IRazorTemplate Template
        {
            get { return _template; }
        }
        public string Name() { return _template.Name(); }
        public IRazorTemplate Master { get; set; }
        public IViewFile ViewFile { get; set; }
        public Type ViewModel { get; set; }
        public string ViewPath { get { return _template.ViewPath; } }
        public string RelativePath() { return _template.RelativePath(); }

        public bool HasViewModel()
        {
            return ViewModel != null;
        }

        public bool IsCurrent()
        {
            return ViewFile.IsCurrent();
            //var isCurrent = Master == null
            //                    ? ViewFile.IsCurrent()
            //                    : ViewFile.IsCurrent() && Master.Descriptor.IsCurrent();
            //return isCurrent;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;

            hashCode ^= _template.GetHashCode();

            if (Master != null)
                hashCode ^= Master.GetHashCode();

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var that = obj as ViewDescriptor;

            if (that == null || GetType() != that.GetType())
                return false;

            return _template == that._template && Master == that.Master;
        }
    }

    public class NulloDescriptor : IRazorDescriptor
    {
        public string Name { get { return "Template"; } }

        public Type ViewModel { get; set; }

        public string RelativePath()
        {
            return null;
        }

        public IRazorTemplate Template
        {
            get { return null; }
        }

        public IRazorTemplate Master
        {
            get { return null; }
        }

        public IViewFile ViewFile
        {
            get { return null; }
        }

        public bool IsCurrent()
        {
            return false;
        }
    }
}