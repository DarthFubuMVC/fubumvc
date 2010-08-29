using FubuMVC.Core.Registration.Nodes;
using Spark;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Routing;
using Spark.Compiler;
using Spark.FileSystem;
using Spark.Web.FubuMVC.Extensions;
using Spark.Web.FubuMVC.ViewCreation;
using Spark.Web.FubuMVC.ViewLocation;
using Microsoft.Practices.ServiceLocation;
using FubuMVC.Core.View;

namespace Spark.Web.FubuMVC
{
    public class SparkViewFactory
    {
        private readonly Dictionary<BuildDescriptorParams, ISparkViewEntry> _cache = new Dictionary<BuildDescriptorParams, ISparkViewEntry>();
        private ICacheServiceProvider _cacheServiceProvider;
        private IDescriptorBuilder _descriptorBuilder;
        private ISparkViewEngine _engine;
        private IServiceLocator _serviceLocator;

        public SparkViewFactory(ISparkSettings settings, IServiceLocator serviceLocator)
        {
            Settings = settings ?? (ISparkSettings) ConfigurationManager.GetSection("spark") ?? new SparkSettings();
            _serviceLocator = serviceLocator;
        }

        public IViewFolder ViewFolder
        {
            get { return Engine.ViewFolder; }
            set { Engine.ViewFolder = value; }
        }

        public IDescriptorBuilder DescriptorBuilder
        {
            get
            {
                return _descriptorBuilder ??
                       Interlocked.CompareExchange(ref _descriptorBuilder, new FubuDescriptorBuilder(Engine), null) ??
                       _descriptorBuilder;
            }
            set { _descriptorBuilder = value; }
        }

        public ISparkSettings Settings { get; set; }

        public ISparkViewEngine Engine
        {
            get
            {
                if (_engine == null)
                    SetEngine(new SparkViewEngine(Settings));

                return _engine;
            }
            set { SetEngine(value); }
        }

        public ICacheServiceProvider CacheServiceProvider
        {
            get
            {
                return _cacheServiceProvider ??
                       Interlocked.CompareExchange(ref _cacheServiceProvider, new DefaultCacheServiceProvider(), null) ??
                       _cacheServiceProvider;
            }
            set { _cacheServiceProvider = value; }
        }

        public void SetEngine(ISparkViewEngine engine)
        {
            _descriptorBuilder = null;
            _engine = engine;
            if (_engine != null)
            {
                _engine.DefaultPageBaseType = typeof (SparkView).FullName;
            }
        }

        public SparkViewDescriptor CreateDescriptor(ActionContext actionContext, string viewName, string masterName, bool findDefaultMaster, ICollection<string> searchedLocations)
        {
            string targetNamespace = actionContext.ActionNamespace;

            string actionName = actionContext.RouteData.GetRequiredString("controller");

            return DescriptorBuilder.BuildDescriptor(
                new BuildDescriptorParams(
                    targetNamespace,
                    actionName,
                    viewName,
                    masterName,
                    findDefaultMaster,
                    DescriptorBuilder.GetExtraParameters(actionContext)),
                searchedLocations);
        }

        public Assembly Precompile(SparkBatchDescriptor batch)
        {
            return Engine.BatchCompilation(batch.OutputAssembly, CreateDescriptors(batch));
        }

        public List<SparkViewDescriptor> CreateDescriptors(SparkBatchDescriptor batch)
        {
            var descriptors = new List<SparkViewDescriptor>();
            foreach (SparkBatchEntry entry in batch.Entries)
                descriptors.AddRange(CreateDescriptors(entry));
            return descriptors;
        }

        public IList<SparkViewDescriptor> CreateDescriptors(SparkBatchEntry entry)
        {
            var descriptors = new List<SparkViewDescriptor>();

            string controllerName = entry.ControllerType.Name.RemoveSuffix("Controller");

            var viewNames = new List<string>();
            IList<string> includeViews = entry.IncludeViews;
            if (includeViews.Count == 0)
                includeViews = new[] {"*"};

            foreach (string include in includeViews)
            {
                if (include.EndsWith("*"))
                {
                    foreach (string fileName in ViewFolder.ListViews(controllerName))
                    {
                        if (!string.Equals(Path.GetExtension(fileName), ".spark", StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        string potentialMatch = Path.GetFileNameWithoutExtension(fileName);
                        if (!potentialMatch.Matches(include))
                            continue;

                        bool isExcluded = false;
                        foreach (string exclude in entry.ExcludeViews)
                        {
                            if (!potentialMatch.Matches(exclude.RemoveSuffix(".spark")))
                                continue;

                            isExcluded = true;
                            break;
                        }
                        if (!isExcluded)
                            viewNames.Add(potentialMatch);
                    }
                }
                else
                {
                    // explicitly included views don't test for exclusion
                    viewNames.Add(include.RemoveSuffix(".spark"));
                }
            }

            foreach (string viewName in viewNames)
            {
                if (entry.LayoutNames.Count == 0)
                {
                    descriptors.Add(CreateDescriptor(
                                        entry.ControllerType.Namespace,
                                        controllerName,
                                        viewName,
                                        null /*masterName*/,
                                        true));
                }
                else
                {
                    foreach (var masterName in entry.LayoutNames)
                    {
                        descriptors.Add(CreateDescriptor(
                                            entry.ControllerType.Namespace,
                                            controllerName,
                                            viewName,
                                            string.Join(" ", masterName.ToArray()),
                                            false));
                    }
                }
            }

            return descriptors;
        }

        public SparkViewDescriptor CreateDescriptor(string targetNamespace, string controllerName, string viewName, string masterName, bool findDefaultMaster)
        {
            var searchedLocations = new List<string>();
            SparkViewDescriptor descriptor = DescriptorBuilder.BuildDescriptor(
                new BuildDescriptorParams(
                    targetNamespace /*areaName*/,
                    controllerName,
                    viewName,
                    masterName,
                    findDefaultMaster, null),
                searchedLocations);

            if (descriptor == null)
            {
                throw new CompilerException("Unable to find templates at " +
                                            string.Join(", ", searchedLocations.ToArray()));
            }
            return descriptor;
        }

        private ViewEngineResult BuildResult(HttpContextBase httpContext, ISparkViewEntry entry)
        {
            ISparkView view = entry.CreateInstance();
            if (view is SparkView)
            {
                var sparkView = (SparkView) view;
                sparkView.ResourcePathManager = Engine.ResourcePathManager;
                sparkView.CacheService = CacheServiceProvider.GetCacheService(httpContext);
            }
            var page = view as IFubuPage;
            if (page != null)
                page.ServiceLocator = _serviceLocator;

            return new ViewEngineResult(view, this);
        }

        public ViewEngineResult FindView(ActionContext actionContext, string viewName, string masterName)
        {
            return FindViewInternal(actionContext, viewName, masterName, true, false);
        }

        public ISparkView FindView(HttpContextBase httpContext, string actionNamespace, string actionName, string viewName, string masterName)
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", actionName.RemoveSuffix("Controller"));
            var actionContext = new ActionContext(httpContext, routeData, actionNamespace);

            ViewEngineResult viewResult = FindView(actionContext, viewName, masterName);
            return viewResult.View;
        }

        public virtual ViewEngineResult FindPartialView(ActionContext actionContext, string partialViewName)
        {
            return FindViewInternal(actionContext, partialViewName, null /*masterName*/, false, false);
        }

        public SparkViewToken GetViewToken(ActionCall call, string controllerName, string viewName, LanguageType languageType)
        {
            var searchedLocations = new List<string>();

            var descriptorParams = new BuildDescriptorParams("", controllerName, viewName, String.Empty, false, null);
            var descriptor = DescriptorBuilder.BuildDescriptor(descriptorParams, searchedLocations);
            if (descriptor == null)
                throw new CompilerException(String.Format(
                    "View '{0}' could not be found in any of the following locations: {1}", viewName, string.Join(", ", searchedLocations)));
            descriptor.Language = languageType;

            return new SparkViewToken(call, descriptor, call.Method.Name);
        }

        private ViewEngineResult FindViewInternal(ActionContext actionContext, string viewName, string masterName, bool findDefaultMaster, bool useCache)
        {
            var searchedLocations = new List<string>();
            string targetNamespace = actionContext.ActionNamespace;

            string controllerName = actionContext.RouteData.GetRequiredString("controller");

            var descriptorParams = new BuildDescriptorParams(
                targetNamespace,
                controllerName,
                viewName,
                masterName,
                findDefaultMaster,
                DescriptorBuilder.GetExtraParameters(actionContext));

            ISparkViewEntry entry;
            if (useCache)
            {
                if (TryGetCacheValue(descriptorParams, out entry) && entry.IsCurrent())
                {
                    return BuildResult(actionContext.HttpContext, entry);
                }
                return new ViewEngineResult(new List<string> { "Cache" });
            }

            SparkViewDescriptor descriptor = DescriptorBuilder.BuildDescriptor(
                descriptorParams,
                searchedLocations);

            if (descriptor == null)
                return new ViewEngineResult(searchedLocations);

            entry = Engine.CreateEntry(descriptor);
            SetCacheValue(descriptorParams, entry);
            return BuildResult(actionContext.HttpContext, entry);
        }

        private bool TryGetCacheValue(BuildDescriptorParams descriptorParams, out ISparkViewEntry entry)
        {
            lock (_cache) return _cache.TryGetValue(descriptorParams, out entry);
        }

        private void SetCacheValue(BuildDescriptorParams descriptorParams, ISparkViewEntry entry)
        {
            lock (_cache) _cache[descriptorParams] = entry;
        }
    }

    public class ViewEngineResult
    {
        public ViewEngineResult(ISparkView view, SparkViewFactory factory)
        {
            View = view;
            Factory = factory;
        }

        public ViewEngineResult(List<string> searchedLocations)
        {
            string locations = string.Empty;
            searchedLocations.ForEach(loc => locations += string.Format("{0} ", loc));
            throw new ConfigurationErrorsException(string.Format("The view could not be in any of the following locations: {0}", locations));
        }

        public ISparkView View { get; set; }
        public SparkViewFactory Factory { get; set; }
    }
}