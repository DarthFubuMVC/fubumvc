using System;
using System.Collections.Generic;
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
    }

    public interface ISparkItemBinder
    {
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

        public void Bind(SparkItem item, BindContext context)
        {
            var masterName = context.Master ?? MasterName;                        
            if (masterName.IsEmpty()) return;

            item.Master = _sharedItemLocator.LocateSpark(masterName, item, context.AvailableItems);
            if (item.Master == null)
            {
                // Log -> Spark compiler is about to blow up. // context.Observer.??
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

        public void Bind(SparkItem item, BindContext context)
        {
            item.ViewModelType = _typeResolver.ResolveType(context.ViewModelType);
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