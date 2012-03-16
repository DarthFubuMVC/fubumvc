using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public class MasterAttacher : ISharingAttacher<ITemplate>
    {
        private readonly IParsingRegistrations<ITemplate> _registrations;
        private readonly ISharedTemplateLocator _templateLocator;

        private const string FallbackMaster = "Application";
        public string MasterName { get; set; }

        public MasterAttacher(IParsingRegistrations<ITemplate> registrations, ISharedTemplateLocator templateLocator)
        {
            _registrations = registrations;
            _templateLocator = templateLocator;

            MasterName = FallbackMaster;
        }

        public bool CanAttach(IAttachRequest<ITemplate> request)
        {
            var descriptor = request.Template.Descriptor as SparkDescriptor;
            var parsing = _registrations.ParsingFor(request.Template);

            return descriptor != null
                && descriptor.Master == null
                && (descriptor.HasViewModel() || parsing.Master.IsNotEmpty())
                && !request.Template.IsPartial()
                && parsing.Master != string.Empty;
        }

        public void Attach(IAttachRequest<ITemplate> request)
        {
            var template = request.Template;
            var tracer = request.Logger;

            var masterName = _registrations.ParsingFor(template).Master ?? MasterName;
            var master = _templateLocator.LocateMaster(masterName, template);

            if (master == null)
            {
                var notFound = "Expected master page [{0}] not found.".ToFormat(masterName);
                tracer.Log(template, notFound);
                return;
            }

            template.Descriptor.As<SparkDescriptor>().Master = master;
            var found = "Master page [{0}] found at {1}".ToFormat(masterName, master.FilePath);
            tracer.Log(template, found);
        }
    }

    public class BindingsAttacher : ISharingAttacher<ITemplate>
    {
        private readonly ISharedTemplateLocator _templateLocator;

        private const string FallbackBindingsName = "bindings";
        public string BindingsName { get; set; }

        public BindingsAttacher(ISharedTemplateLocator templateLocator)
        {
            _templateLocator = templateLocator;
            BindingsName = FallbackBindingsName;
        }

        public bool CanAttach(IAttachRequest<ITemplate> request)
        {
            var descriptor = request.Template.Descriptor as SparkDescriptor;
            
            return descriptor != null 
                && descriptor.Bindings.Count() == 0;
        }

        public void Attach(IAttachRequest<ITemplate> request)
        {
            var target = request.Template;
            var logger = request.Logger;
            var descriptor = target.Descriptor.As<SparkDescriptor>();

            _templateLocator.LocateBindings(BindingsName, target).Each(template =>
            {
                descriptor.AddBinding(template);
                var msg = "Binding attached : {0}".ToFormat(template.FilePath);
                logger.Log(target, msg);
            });
        }
    }
}