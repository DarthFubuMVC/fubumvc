using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public class LayoutAttacher : ISharingAttacher<IRazorTemplate>
    {
        private readonly IParsingRegistrations<IRazorTemplate> _parsing;
        private readonly ISharedTemplateLocator _sharedTemplateLocator;

        private const string FallbackLayout = "_Layout";
        public string LayoutName { get; set; }

        public LayoutAttacher(IParsingRegistrations<IRazorTemplate> parsing, ISharedTemplateLocator sharedTemplateLocator)
        {
            _parsing = parsing;
            _sharedTemplateLocator = sharedTemplateLocator;
            LayoutName = FallbackLayout;
        }

        public bool CanAttach(IAttachRequest<IRazorTemplate> request)
        {
            var descriptor = request.Template.Descriptor as RazorViewDescriptor;
            var parsing = _parsing.ParsingFor(request.Template);

            return descriptor != null
                && descriptor.Master == null
                && (parsing.ViewModelType.IsNotEmpty() || parsing.Master.IsNotEmpty())
                && parsing.Master != string.Empty;
        }

        public void Attach(IAttachRequest<IRazorTemplate> request)
        {
            var template = request.Template;
            var tracer = request.Logger;

            var layoutName = _parsing.ParsingFor(template).Master ?? LayoutName;
            var layout = _sharedTemplateLocator.LocateMaster(layoutName, template);

            if (layout == null)
            {
                var notFound = "Expected layout [{0}] not found.".ToFormat(layoutName);
                tracer.Log(template, notFound);
                return;
            }

            template.Descriptor.As<RazorViewDescriptor>().Master = layout;
            var found = "Layout [{0}] found at {1}".ToFormat(layoutName, layout.FilePath);
            tracer.Log(template, found);
        }
    }
}