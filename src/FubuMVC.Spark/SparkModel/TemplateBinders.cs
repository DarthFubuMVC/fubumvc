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
        IEnumerable<ITemplate> Templates { get; }
        ISparkLogger Logger { get; }
    }

    public class BindRequest : IBindRequest
    {
        public ITemplate Target { get; set; }

		public string Master { get; set; }
		public string ViewModelType { get; set; }
		public IEnumerable<string> Namespaces { get; set; }

        public TypePool Types { get; set; }
        public IEnumerable<ITemplate> Templates { get; set; }
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
		private readonly ISharedTemplateLocator _sharedTemplateLocator;
		private const string FallbackMaster = "Application";
		public string MasterName { get; set; }

		public MasterPageBinder() : this(new SharedTemplateLocator()) {}
		public MasterPageBinder(ISharedTemplateLocator sharedTemplateLocator)
		{
			_sharedTemplateLocator = sharedTemplateLocator;
			MasterName = FallbackMaster;
		}

        public bool CanBind(IBindRequest request)
        {
            var template = request.Target;
            return template.Descriptor is ViewDescriptor && request.Master != string.Empty;
        }

        public void Bind(IBindRequest request)
        {
            var template = request.Target;
			var tracer = request.Logger;
			var masterName = request.Master ?? MasterName;

			var master = _sharedTemplateLocator.LocateTemplate(masterName, template, request.Templates);
			
			if(master == null)
			{
				tracer.Log(template, "Expected master page [{0}] not found.", masterName);
				return;
			}

			if(master.FilePath == template.FilePath)
			{
				tracer.Log(template, "Master page skipped on itself.", masterName);
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
		    var viewModelType = types.Count() == 1 ? types.First() : null;
		    descriptor.ViewModel = viewModelType;
            
            request.Logger.Log(template, "View model type is : [{0}]", descriptor.ViewModel);
		}
	}

    // TODO : We need to utilize same algorithm that is used for locating shared paths. 

    //public class ReachableBindingsBinder : ISparkTemplateBinder
    //{
    //    private const string Bindings = "bindings.xml";

    //    public bool CanBind(ITemplate template, IBindContext context)
    //    {
    //        return template.Descriptor is ViewDescriptor;
    //    }

    //    public void Bind(ITemplate template, IBindContext context)
    //    {
    //        var descriptor = template.Descriptor.As<ViewDescriptor>();
    //        var candidates = context.AvailableTemplates
    //            .Where(x => x.IsXml()).Where(x => x.Name() == Bindings)
    //            .ToList();
    //        var bindings = getTemplates(template, candidates);
    //        bindings.Each(descriptor.AddBinding);

    //    }
    //    private static IEnumerable<ITemplate> getTemplates(ITemplate template, IEnumerable<ITemplate> candidates)
    //    {
    //        var directory = Path.GetDirectoryName(template.ViewPath) ?? string.Empty;
    //        var templates = new List<ITemplate>();
    //        do
    //        {
    //            var nearestPath = Path.Combine(directory, Bindings);
    //            var sharedPath = Path.Combine(directory, Constants.Shared, Bindings);
    //            templates.AddRange(candidates.Where(x => x.ViewPath == nearestPath || x.ViewPath == sharedPath));
    //            if (directory.Length > 0)
    //            {
    //                directory = Path.GetDirectoryName(directory);
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        } while (directory != null);
    //        return templates;
    //    }
    //}
}