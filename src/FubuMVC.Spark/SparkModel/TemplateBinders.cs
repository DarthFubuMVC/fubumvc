using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;

namespace FubuMVC.Spark.SparkModel
{
    public interface IBindRequest
    {
        ITemplate Target { get; }

        string Master { get; }
        string ViewModelType { get; }
        IEnumerable<string> Namespaces { get; }

        TypePool Types { get; }
        ITemplates Templates { get; }
        ISparkLogger Logger { get; }
    }

    public class BindRequest : IBindRequest
    {
        public ITemplate Target { get; set; }

		public string Master { get; set; }
		public string ViewModelType { get; set; }
		public IEnumerable<string> Namespaces { get; set; }

        public TypePool Types { get; set; }
        public ITemplates Templates { get; set; }
		public ISparkLogger Logger { get; set; }
	}

	public interface ITemplateBinder
	{
        bool CanBind(IBindRequest request);
        void Bind(IBindRequest request);
	}

    public class ViewDescriptorBinder : ITemplateBinder
    {
        public bool CanBind(IBindRequest request)
        {
            var template = request.Target;

            return template.IsSparkView() 
                && !template.IsPartial() 
                && request.ViewModelType.IsNotEmpty();
        }

        public void Bind(IBindRequest request)
        {
            request.Target.Descriptor = new ViewDescriptor(request.Target);
        }
    }

    public class MasterPageBinder : ITemplateBinder
	{
		private readonly ITemplateLocator _templateLocator;
		private const string FallbackMaster = "Application";
		public string MasterName { get; set; }

		public MasterPageBinder() : this(new TemplateLocator()) {}
		public MasterPageBinder(ITemplateLocator templateLocator)
		{
			_templateLocator = templateLocator;
			MasterName = FallbackMaster;
		}

        public bool CanBind(IBindRequest request)
        {
            return request.Target.Descriptor is ViewDescriptor 
				&& request.Master != string.Empty;
        }

        public void Bind(IBindRequest request)
        {
            var template = request.Target;
			var tracer = request.Logger;
			var masterName = request.Master ?? MasterName;
			
			var master = _templateLocator.LocateMaster(masterName, template, request.Templates);
			
			if(master == null)
			{
				tracer.Log(template, "Expected master page [{0}] not found.", masterName);
				return;
			}

		    template.Descriptor.As<ViewDescriptor>().Master = master;
			tracer.Log(template, "Master page [{0}] found at {1}", masterName, master.FilePath);
		}		
	}

	public class ViewModelBinder : ITemplateBinder
	{
        public bool CanBind(IBindRequest request)
		{
			return request.Target.Descriptor is ViewDescriptor 
                && request.ViewModelType.IsNotEmpty();
		}

        public void Bind(IBindRequest request)
        {
            var template = request.Target;
            var descriptor = template.Descriptor.As<ViewDescriptor>();

            var types = request.Types.TypesWithFullName(request.ViewModelType);
            if(types.Count() != 1)
            {
                var candidates = types.Select(x => x.AssemblyQualifiedName).Join(", ");
                const string msg = "Unable to set view model type : {0} - candidates were : {1}";
                
                request.Logger.Log(template, msg, candidates);
                return;
            }

            descriptor.ViewModel = types.First();            
            request.Logger.Log(template, "View model type is : [{0}]", descriptor.ViewModel);
		}
	}

    public class ReachableBindingsBinder : ITemplateBinder
    {
        private readonly ITemplateLocator _templateLocator;
        private const string FallbackBindingsName = "bindings";
        public string BindingsName { get; set; }

        public ReachableBindingsBinder() : this(new TemplateLocator()) {}
        public ReachableBindingsBinder(ITemplateLocator templateLocator)
        {
            _templateLocator = templateLocator;
            BindingsName = FallbackBindingsName;
        }

        public bool CanBind(IBindRequest request)
        {
            return request.Target.Descriptor is ViewDescriptor;
        }

        public void Bind(IBindRequest request)
        {
            var target = request.Target;
            var logger = request.Logger;
            var templates = request.Templates;
            var descriptor = target.Descriptor.As<ViewDescriptor>();

            _templateLocator.LocateBindings(BindingsName, target, templates)
            .Each(template =>
            {
                descriptor.AddBinding(template);
                logger.Log(target, "Binding attached : {0}", template.FilePath);
            });
        }
    }
}