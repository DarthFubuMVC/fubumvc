using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public class SharingAttacherActivator : IActivator
    {
        private readonly ITemplateRegistry _templates;
        private readonly IEnumerable<ISharingAttacher> _attachers;

        public SharingAttacherActivator(ITemplateRegistry templates, IEnumerable<ISharingAttacher> attachers)
        {
            _templates = templates;
            _attachers = attachers;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _templates.AllTemplates().Each(t =>
            {                
                var context = new AttachRequest
                {
                    Template = t,
                    Logger = SparkLogger.Default()
                };

                _attachers.Where(a => a.CanAttach(context)).Each(a => a.Attach(context));
            });
        }
    }

    public interface IAttachRequest
    {
        ITemplate Template { get; }
        ISparkLogger Logger { get; }
    }

    public class AttachRequest : IAttachRequest
    {
        public ITemplate Template { get; set; }
        public ISparkLogger Logger { get; set; }
    }

    public interface ISharingAttacher
    {
        bool CanAttach(IAttachRequest request);
        void Attach(IAttachRequest request);
    }

    public class MasterAttacher : ISharingAttacher
    {
        private readonly IParsingRegistrations _registrations;
        private readonly ISharedTemplateLocator _templateLocator;

        private const string FallbackMaster = "Application";
        public string MasterName { get; set; }

        public MasterAttacher(IParsingRegistrations registrations, ISharedTemplateLocator templateLocator)
        {
            _registrations = registrations;
            _templateLocator = templateLocator;

            MasterName = FallbackMaster;
        }

        public bool CanAttach(IAttachRequest request)
        {
            var descriptor = request.Template.Descriptor as ViewDescriptor;
            var parsing = _registrations.ParsingFor(request.Template);

            return descriptor != null && descriptor.Master == null
                && (descriptor.HasViewModel() || parsing.Master.IsNotEmpty())
                && !request.Template.IsPartial() && parsing.Master != string.Empty;            
        }

        public void Attach(IAttachRequest request)
        {
            var template = request.Template;
            var tracer = request.Logger;

            var masterName = _registrations.ParsingFor(template).Master ?? MasterName;
            var master = _templateLocator.LocateMaster(masterName, template);

            if (master == null)
            {
                tracer.Log(template, "Expected master page [{0}] not found.", masterName);
                return;
            }

            template.Descriptor.As<ViewDescriptor>().Master = master;
            tracer.Log(template, "Master page [{0}] found at {1}", masterName, master.FilePath);
        }
    }

    public class BindingsAttacher : ISharingAttacher
    {
        private readonly ISharedTemplateLocator _templateLocator;

        private const string FallbackBindingsName = "bindings";
        public string BindingsName { get; set; }

        public BindingsAttacher(ISharedTemplateLocator templateLocator)
        {
            _templateLocator = templateLocator;
            BindingsName = FallbackBindingsName;
        }

        public bool CanAttach(IAttachRequest request)
        {
            var descriptor = request.Template.Descriptor as ViewDescriptor;
            return descriptor != null && descriptor.Bindings.Count() == 0;
        }

        public void Attach(IAttachRequest request)
        {
            var target = request.Template;
            var logger = request.Logger;
            var descriptor = target.Descriptor.As<ViewDescriptor>();

            _templateLocator.LocateBindings(BindingsName, target).Each(template =>
            {
                descriptor.AddBinding(template);
                logger.Log(target, "Binding attached : {0}", template.FilePath);
            });
        }
    }
}