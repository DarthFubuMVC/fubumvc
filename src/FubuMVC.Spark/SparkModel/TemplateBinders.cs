using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Registration;

namespace FubuMVC.Spark.SparkModel
{

    public interface IBindRequest
    {
        ITemplate Target { get; }

        string Master { get; }
        string ViewModelType { get; }
        IEnumerable<string> Namespaces { get; }

        TypePool Types { get; }
        ITemplateRegistry TemplateRegistry { get; }
        ISparkLogger Logger { get; }
    }

    public class BindRequest : IBindRequest
    {
        public ITemplate Target { get; set; }

        public string Master { get; set; }
        public string ViewModelType { get; set; }
        public IEnumerable<string> Namespaces { get; set; }

        public TypePool Types { get; set; }
        public ITemplateRegistry TemplateRegistry { get; set; }
        public ISparkLogger Logger { get; set; }
    }

    public interface ITemplateBinder
    {
        bool CanBind(IBindRequest request);
        void Bind(IBindRequest request);
    }

    // NOTE: This one is needed before activation
    public class ViewDescriptorBinder : ITemplateBinder
    {
        public bool CanBind(IBindRequest request)
        {
            var template = request.Target;
            return !(template.Descriptor is ViewDescriptor) && template.IsSparkView();
        }

        public void Bind(IBindRequest request)
        {
            request.Target.Descriptor = new ViewDescriptor(request.Target);
        }
    }

    // NOTE: Not needed before activation
    public class MasterPageBinder : ITemplateBinder
    {
        private readonly ISharedTemplateLocator _sharedTemplateLocator;
        private const string FallbackMaster = "Application";
        public string MasterName { get; set; }

        public MasterPageBinder() : this(new SharedTemplateLocator()) { }
        public MasterPageBinder(ISharedTemplateLocator sharedTemplateLocator)
        {
            _sharedTemplateLocator = sharedTemplateLocator;
            MasterName = FallbackMaster;
        }

        public bool CanBind(IBindRequest request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor;

            return descriptor != null
                && descriptor.Master == null
                && (request.ViewModelType.IsNotEmpty() || request.Master.IsNotEmpty())
                && !request.Target.IsPartial()
                && request.Master != string.Empty;
        }

        public void Bind(IBindRequest request)
        {
            var template = request.Target;
            var tracer = request.Logger;
            var masterName = request.Master ?? MasterName;

            var master = _sharedTemplateLocator.LocateMaster(masterName, template, request.TemplateRegistry);

            if (master == null)
            {
                tracer.Log(template, "Expected master page [{0}] not found.", masterName);
                return;
            }

            template.Descriptor.As<ViewDescriptor>().Master = master;
            tracer.Log(template, "Master page [{0}] found at {1}", masterName, master.FilePath);
        }
    }

    // NOTE: This one is needed before activation
    public class GenericViewModelBinder : ITemplateBinder
    {
        public bool CanBind(IBindRequest request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && !request.Target.IsPartial()
                   && request.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(request.ViewModelType);
        }

        public void Bind(IBindRequest request)
        {
            var logger = request.Logger;
            var template = request.Target;

            var genericParser = new GenericParser(request.Types.Assemblies);
            var viewModel = genericParser.Parse(request.ViewModelType);

            if (viewModel != null)
            {
                var descriptor = template.Descriptor.As<ViewDescriptor>();
                descriptor.ViewModel = viewModel;
                logger.Log(template, "Generic view model type is : {0}", descriptor.ViewModel);
                return;
            }

            genericParser.ParseErrors.Each(error => logger.Log(template, error));
        }
    }

    // NOTE: This one is needed before activation
    public class ViewModelBinder : ITemplateBinder
    {
        public bool CanBind(IBindRequest request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && !request.Target.IsPartial()
                   && request.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(request.ViewModelType) == false;
        }

        public void Bind(IBindRequest request)
        {
            var logger = request.Logger;
            var template = request.Target;
            var descriptor = template.Descriptor.As<ViewDescriptor>();

            var types = request.Types.TypesWithFullName(request.ViewModelType);
            var typeCount = types.Count();

            if (typeCount == 1)
            {
                descriptor.ViewModel = types.First();
                logger.Log(template, "View model type is : [{0}]", descriptor.ViewModel);

                return;
            }

            logger.Log(template, "Unable to set view model type : {0}", request.ViewModelType);

            if (typeCount > 1)
            {
                var candidates = types.Select(x => x.AssemblyQualifiedName).Join(", ");
                logger.Log(template, "Type ambiguity on: {0}", candidates);
            }
        }
    }

    // NOTE: Not needed before activation
    public class ReachableBindingsBinder : ITemplateBinder
    {
        private readonly ISharedTemplateLocator _sharedTemplateLocator;
        private const string FallbackBindingsName = "bindings";
        public string BindingsName { get; set; }

        public ReachableBindingsBinder() : this(new SharedTemplateLocator()) { }
        public ReachableBindingsBinder(ISharedTemplateLocator sharedTemplateLocator)
        {
            _sharedTemplateLocator = sharedTemplateLocator;
            BindingsName = FallbackBindingsName;
        }

        public bool CanBind(IBindRequest request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor;

            return descriptor != null && descriptor.Bindings.Count() == 0;
        }

        public void Bind(IBindRequest request)
        {
            var target = request.Target;
            var logger = request.Logger;
            var templates = request.TemplateRegistry;
            var descriptor = target.Descriptor.As<ViewDescriptor>();

            var bindings = _sharedTemplateLocator.LocateBindings(BindingsName, target, templates);
            bindings.Each(template =>
            {
                descriptor.AddBinding(template);
                logger.Log(target, "Binding attached : {0}", template.FilePath);
            });
        }
    }
}
