using FubuCore;

namespace FubuMVC.Core.View.Model
{
    public class MasterAttacher<T> : ISharingAttacher<T> where T : ITemplateFile
    {
        private readonly ISharedTemplateLocator<T> _sharedTemplateLocator;

        private const string FallbackMaster = "Application";
        public string MasterName { get; set; }

        public MasterAttacher(ISharedTemplateLocator<T> sharedTemplateLocator)
        {
            _sharedTemplateLocator = sharedTemplateLocator;
            MasterName = FallbackMaster;
        }

        public bool CanAttach(IAttachRequest<T> request)
        {
            var descriptor = request.Template.Descriptor as ViewDescriptor<T>;
            var parsing =request.Template.Parsing;

            return descriptor != null
                   && descriptor.Master == null
                   && (descriptor.Template.ViewModel != null || parsing.Master.IsNotEmpty())
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

            var layoutName = template.Parsing.Master ?? MasterName;
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