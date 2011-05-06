using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;

namespace FubuMVC.Spark.SparkModel
{
    // TODO : Reconsider this
	public class BindContext
	{
		public string Master { get; set; }
		public string ViewModelType { get; set; }
		public IEnumerable<string> Namespaces { get; set; }

        public TypePool TypePool { get; set; }
		public IEnumerable<SparkItem> AvailableItems { get; set; }
		public ISparkTracer Tracer { get; set; }
	}

	public interface ISparkItemBinder
	{
		bool CanBind(SparkItem item, BindContext context);
		void Bind(SparkItem item, BindContext context);
	}

	public class MasterPageBinder : ISparkItemBinder
	{
		private readonly ISharedItemLocator _sharedItemLocator;
		private const string FallbackMaster = "Application";

		public string MasterName { get; set; }

		public MasterPageBinder() : this(new SharedItemLocator()) {}
		public MasterPageBinder(ISharedItemLocator sharedItemLocator)
		{
			_sharedItemLocator = sharedItemLocator;
			MasterName = FallbackMaster;
		}

		public bool CanBind(SparkItem item, BindContext context)
		{		
			return context.Master != string.Empty && item.IsSparkView() && !item.IsPartial();
		}

		public void Bind(SparkItem item, BindContext context)
		{
			var tracer = context.Tracer;
			var masterName = context.Master ?? MasterName;

			var master = _sharedItemLocator.LocateItem(masterName, item, context.AvailableItems);
			
			if(master == null)
			{
				tracer.Trace(item, "Expected master page [{0}] not found.", masterName);
				return;
			}

			if(master.FilePath == item.FilePath)
			{
				tracer.Trace(item, "Master page skipped because it is the item.", masterName);
				return;
			}
			            
			item.Master = master;
			tracer.Trace(item, "Master page [{0}] found at {1}", masterName, master.FilePath);
		}		
	}

	public class ViewModelBinder : ISparkItemBinder
	{	
		public bool CanBind(SparkItem item, BindContext context)
		{
			return context.ViewModelType.IsNotEmpty();
		}

		public void Bind(SparkItem item, BindContext context)
		{
			// TODO: We could account for <use namespace="..."/>
            var types = context.TypePool.TypesWithFullName(context.ViewModelType);
            item.ViewModelType = types.Count() == 1 ? types.First() : null;
			context.Tracer.Trace(item, "View model type is : [{0}]", item.ViewModelType);
		}
	}
}