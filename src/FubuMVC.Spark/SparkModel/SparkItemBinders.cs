using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bottles;
using FubuCore;
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

		bool CanBind(SparkItem item, BindContext context)
		{
			return Path.GetDirectoryName(item.DirectoryPath()) != Constants.Shared 
				|| context.Master != string.Empty;
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

		bool CanBind(SparkItem item, BindContext context)
		{
			return context.ViewModelType.IsNotEmpty();
		}

        public void Bind(SparkItem item, BindContext context)
        {
            item.ViewModelType = _typeResolver.ResolveType(context.ViewModelType);
            context.Tracer.Trace(item, "View model type is : [{0}]", item.ViewModelType);
        }
    }
	
	// REVIEW: I think we are not gaining much from ITypeResolver and ITypeResolverStrategy in our usage.
	//         All the type hookey pokey in here needs simplification and belongs in some other place..	
    //         A better and cleaner way of getting relevant types from fubu itself would be desirable. 
	//         TypePool via IViewFacility is only available via FindViews and too late.
	
    // TODO : kill this frankenstein cookie monster.
    public class FullTypeNameStrategy : ITypeResolverStrategy
    {
        private readonly string _binPath;
        private readonly Func<string, bool> _assemblyFilter;

        private readonly Lazy<TypePool> _typePool;

        // Rip this apart, split out, find a more simple way. Ask JDM.
        public FullTypeNameStrategy() : this(AppDomain.CurrentDomain.RelativeSearchPath, excludedAssembly) {}
        public FullTypeNameStrategy(string binPath, Func<string, bool> assemblyFilter)
        {
            _binPath = binPath;
            _assemblyFilter = assemblyFilter;
            
            _typePool = new Lazy<TypePool>(defaultTypePool);
        }

        public IEnumerable<Type> TypesWithFullName(string fullTypeName)
        {
            return _typePool.Value.TypesWithFullName(fullTypeName);
        }

        public bool Matches(object model)
        {
            return model is string;
        }

        public Type ResolveType(object model)
        {
            var typeName = (string)model;
            var types = TypesWithFullName(typeName);
            return types.Count() == 1 ? types.First() : null;
        }
        
        private TypePool defaultTypePool()
        {
            var typePool = new TypePool(Assembly.GetCallingAssembly())
            {
                ShouldScanAssemblies = true
            };

            typePool.AddAssemblies(relevantAssemblies());
            
            return typePool;
        }

        private IEnumerable<Assembly> relevantAssemblies()
        {
            var relevantAssemblies = findAssemblyNames()
                .Where(x => !_assemblyFilter(x)).Distinct().ToList();

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && relevantAssemblies.Contains(x.GetName().Name));
        }

        private IEnumerable<string> findAssemblyNames()
        {
            return new FileSystem().FindAssemblyNames(_binPath);
        }

        private static bool excludedAssembly(string assemblyName)
        {
            return new[]
            {
                "Ionic.Zip",
                "StructureMap",
                "Microsoft.Practices.ServiceLocation",
                "Spark",
                "HtmlTags",
                "FubuMVC.StructureMap",
                "FubuMVC.Spark",
                "FubuCore",
                "Bottles"
            }
            .Contains(assemblyName);
        }
    }
}