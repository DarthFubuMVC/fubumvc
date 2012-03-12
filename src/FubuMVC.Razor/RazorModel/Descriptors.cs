using FubuMVC.Core.View.Model;
using FubuMVC.Razor.FileSystem;

namespace FubuMVC.Razor.RazorModel
{
    public class RazorViewDescriptor : ViewDescriptor<IRazorTemplate>
    {
        private readonly IRazorTemplate _template;
        public RazorViewDescriptor(IRazorTemplate template) : base(template)
        {
            _template = template;
        }

        public IRazorTemplate Template
        {
            get { return _template; }
        }

        public IViewFile ViewFile { get; set; }

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
            var that = obj as RazorViewDescriptor;

            if (that == null || GetType() != that.GetType())
                return false;

            return _template == that._template && Master == that.Master;
        }
    }
}