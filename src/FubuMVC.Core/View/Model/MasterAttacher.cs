using FubuCore;

namespace FubuMVC.Core.View.Model
{
    public class MasterAttacher<T> : ISharingAttacher<T> where T : ITemplateFile
    {
        private readonly IParsingRegistrations<T> _parsing;
        private readonly ISharedTemplateLocator<T> _sharedTemplateLocator;

        private const string FallbackMaster = "Application";
        public string MasterName { get; set; }

        public MasterAttacher(IParsingRegistrations<T> parsing, ISharedTemplateLocator<T> sharedTemplateLocator)
        {
            _parsing = parsing;
            _sharedTemplateLocator = sharedTemplateLocator;
            MasterName = FallbackMaster;
        }

        public bool CanAttach(IAttachRequest<T> request)
        {
            var descriptor = request.Template.Descriptor as ViewDescriptor<T>;
            var parsing = _parsing.ParsingFor(request.Template);

            return descriptor != null
                   && descriptor.Master == null
                   && (descriptor.HasViewModel() || parsing.Master.IsNotEmpty())
                   && !request.Template.IsPartial()
                   && parsing.Master != string.Empty
                   && (!descriptor.Template.Name().EqualsIgnoreCase(MasterName)
                       ||
                       (descriptor.Template.Name().EqualsIgnoreCase(MasterName) && parsing.Master != null &&
                        !parsing.Master.EqualsIgnoreCase(MasterName)));
        }

        public void Attach(IAttachRequest<T> request)
        {
            var template = request.Template;
            var tracer = request.Logger;

            var layoutName = _parsing.ParsingFor(template).Master ?? MasterName;
            var layout = _sharedTemplateLocator.LocateMaster(layoutName, template);

            if (layout == null)
            {
                var notFound = "Expected master [{0}] not found.".ToFormat(layoutName);
                tracer.Log(template, notFound);
                return;
            }

            template.Descriptor.As<ViewDescriptor<T>>().Master = layout;
            var found = "Master [{0}] found at {1}".ToFormat(layoutName, layout.FilePath);
            tracer.Log(template, found);
        }
    }
}