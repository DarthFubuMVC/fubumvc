using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    // TODO : Reconsider this
	public class BindContext
	{
		public string Master { get; set; }
		public string ViewModelType { get; set; }
		public IEnumerable<string> Namespaces { get; set; }

        public TypePool TypePool { get; set; }
        public IEnumerable<ITemplate> AvailableTemplates { get; set; }
		public ISparkTracer Tracer { get; set; }
	}

	public interface ISparkTemplateBinder
	{
		bool CanBind(ITemplate template, BindContext context);
		void Bind(ITemplate template, BindContext context);
	}

    public class ViewDescriptorBinder : ISparkTemplateBinder
    {
        public bool CanBind(ITemplate template, BindContext context)
        {
            return template.IsSparkView() && !template.IsPartial();
        }

        public void Bind(ITemplate template, BindContext context)
        {
           new ViewDescriptor(template);
        }
    }

    public class MasterPageBinder : ISparkTemplateBinder
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

		public bool CanBind(ITemplate template, BindContext context)
		{
            return template.Descriptor is ViewDescriptor && context.Master != string.Empty && template.IsSparkView() && !template.IsPartial();
		}

		public void Bind(ITemplate template, BindContext context)
		{
			var tracer = context.Tracer;
			var masterName = context.Master ?? MasterName;

			var master = _sharedTemplateLocator.LocateTemplate(masterName, template, context.AvailableTemplates);
			
			if(master == null)
			{
				tracer.Trace(template, "Expected master page [{0}] not found.", masterName);
				return;
			}

			if(master.FilePath == template.FilePath)
			{
				tracer.Trace(template, "Master page skipped on itself.", masterName);
				return;
			}
		    template.Descriptor.As<ViewDescriptor>().Master = master;
			tracer.Trace(template, "Master page [{0}] found at {1}", masterName, master.FilePath);
		}		
	}

	public class ViewModelBinder : ISparkTemplateBinder
	{	
		public bool CanBind(ITemplate template, BindContext context)
		{
			return template.Descriptor is ViewDescriptor && context.ViewModelType.IsNotEmpty();
		}

		public void Bind(ITemplate template, BindContext context)
		{
			// TODO: We could account for <use namespace="..."/>
            var types = context.TypePool.TypesWithFullName(context.ViewModelType);
		    var viewModelType = types.Count() == 1 ? types.First() : null;
		    var descriptor = template.Descriptor.As<ViewDescriptor>();
		    descriptor.ViewModel = viewModelType;
            context.Tracer.Trace(template, "View model type is : [{0}]", descriptor.ViewModel);
		}
	}

    public class ReachableBindingsBinder : ISparkTemplateBinder
    {
        private const string Bindings = "bindings.xml";

        public bool CanBind(ITemplate template, BindContext context)
        {
            return template.Descriptor is ViewDescriptor;
        }

        public void Bind(ITemplate template, BindContext context)
        {
            var descriptor = template.Descriptor.As<ViewDescriptor>();
            var candidates = context.AvailableTemplates
                .Where(x => x.IsXml()).Where(x => x.Name() == Bindings)
                .ToList();
            var bindings = getTemplates(template, candidates);
            bindings.Each(descriptor.AddBinding);

        }
        private static IEnumerable<ITemplate> getTemplates(ITemplate template, IEnumerable<ITemplate> candidates)
        {
            var directory = Path.GetDirectoryName(template.ViewPath) ?? string.Empty;
            var templates = new List<ITemplate>();
            do
            {
                var nearestPath = Path.Combine(directory, Bindings);
                var sharedPath = Path.Combine(directory, Constants.Shared, Bindings);
                templates.AddRange(candidates.Where(x => x.ViewPath == nearestPath || x.ViewPath == sharedPath));
                if (directory.Length > 0)
                {
                    directory = Path.GetDirectoryName(directory);
                }
                else
                {
                    break;
                }
            } while (directory != null);
            return templates;
        }
    }
}