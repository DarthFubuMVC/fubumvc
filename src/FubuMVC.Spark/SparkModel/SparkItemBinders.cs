using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bottles;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public class BindContext
    {
        public IEnumerable<string> Namespaces { get; set; }
        public string Master { get; set; }
        public string ViewModelType { get; set; }
        public IEnumerable<SparkItem> AvailableItems { get; set; }
        public ISparkPackageTracer Tracer { get; set; }
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

        public MasterPageBinder() : this(new SharedItemLocator(new[] {Constants.Shared})){}
        public MasterPageBinder(ISharedItemLocator sharedItemLocator)
        {
            _sharedItemLocator = sharedItemLocator;
            MasterName = FallbackMaster;
        }

		public bool CanBind(SparkItem item, BindContext context)
		{
			var sharedFolder = "{0}{1}".ToFormat(Path.DirectorySeparatorChar, Constants.Shared);
			var itemDirectory = item.DirectoryPath();
			
			return !itemDirectory.EndsWith(sharedFolder) && 
				 	context.Master != string.Empty && 
					Path.GetExtension(item.FilePath) == Constants.DotSpark;
		}

        public void Bind(SparkItem item, BindContext context)
        {
			var tracer = context.Tracer;
            var masterName = context.Master;

			if (masterName == null)
            {
                masterName = FallbackMaster;
                tracer.Trace(item, "Using default master page [{0}].", masterName);
            }

            item.Master = _sharedItemLocator.LocateItem(masterName, item, context.AvailableItems);

			if (item.Master == null)
            {
                tracer.Trace(item, "Expected master page [{0}] not found.", masterName);
            }
            else
            {
                tracer.Trace(item, "Master page [{0}] found at {1}", masterName, item.Master.FilePath);
            }
        }
    }

    public class ViewModelBinder : ISparkItemBinder
    {
        private readonly ITypeResolver _typeResolver;
        public ViewModelBinder()
        {
            var resolver = new TypeResolver();
            resolver.AddStrategy<FullTypeNameStrategy>();
            _typeResolver = resolver;
        }

        public ViewModelBinder(ITypeResolver typeResolver)
        {
            _typeResolver = typeResolver;
        }

		public bool CanBind(SparkItem item, BindContext context)
		{
			return context.ViewModelType.IsNotEmpty();
		}

        public void Bind(SparkItem item, BindContext context)
        {
            item.ViewModelType = _typeResolver.ResolveType(context.ViewModelType);
            context.Tracer.Trace(item, "View model type is : [{0}]", item.ViewModelType);
        }
    }
	
	// Note: Would be much better to rely on something like AssemblyScanner
	// (Diagnostics has a simple one, but we need SM like).
    public class FullTypeNameStrategy : ITypeResolverStrategy
    {
        private readonly TypePool _typePool;
		
		public FullTypeNameStrategy() : this(DefaultTypePool()){}
		public FullTypeNameStrategy(TypePool typePool)
		{
			_typePool = typePool;
		}		
						
        public bool Matches(object model)
        {
            return model is string;
        }

        public Type ResolveType(object model)
        {
            var types = _typePool.TypesWithFullName((string)model);            
			return types.Count() == 1 ? types.First() : null;
        }

        public static TypePool DefaultTypePool()
        {			
			var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;			
			var candiateNames = new FileSystem().FindAssemblyNames(binPath).ToList();			
			
			var assemblyFilters = new CompositeFilter<Assembly>();						
			assemblyFilters.Includes += a => candiateNames.Contains(a.GetName().Name);
			assemblyFilters.Excludes += a => a.IsDynamic;

			var assemblies = AppDomain.CurrentDomain.GetAssemblies()
				.Where(assemblyFilters.MatchesAll);
		
			var typePool = new TypePool(Assembly.GetCallingAssembly()) 
			{ 
				ShouldScanAssemblies = true 
			};
			
            typePool.AddAssemblies(assemblies.ToList());
			
			return typePool;
        }		
    }
}