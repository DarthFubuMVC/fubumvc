using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Spark.Compiler;
using Spark.FileSystem;
using Spark.Web.FubuMVC.ViewLocation;

namespace Spark.Web.FubuMVC
{
    public class SparkViewFactory
    {
        private IDescriptorBuilder _descriptorBuilder;
        private ISparkViewEngine _engine;

        public SparkViewFactory() : this(null) { }
        public SparkViewFactory(ISparkSettings settings)
        {
            Settings = settings ?? (ISparkSettings)ConfigurationManager.GetSection("spark") ?? new SparkSettings();
        }

        public string ViewFileExtension
        {
            get { throw new NotImplementedException(); }
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

        public void SetEngine(ISparkViewEngine engine)
        {
            _descriptorBuilder = null;
            _engine = engine;
            if (_engine != null)
            {
                _engine.DefaultPageBaseType = typeof(SparkView).FullName;
            }
        }

        public SparkViewDescriptor CreateDescriptor(ControllerContext controllerContext, string viewName, string masterName, bool findDefaultMaster, ICollection<string> searchedLocations)
        {
            string targetNamespace = controllerContext.Controller.GetType().Namespace;

            string controllerName = controllerContext.RouteData.GetRequiredString("controller");

            return DescriptorBuilder.BuildDescriptor(
                new BuildDescriptorParams(
                    targetNamespace,
                    controllerName,
                    viewName,
                    masterName,
                    findDefaultMaster,
                    DescriptorBuilder.GetExtraParameters(controllerContext)),
                searchedLocations);
        }

        public Assembly Precompile(SparkBatchDescriptor batch)
        {
            return Engine.BatchCompilation(batch.OutputAssembly, CreateDescriptors(batch));
        }

        public List<SparkViewDescriptor> CreateDescriptors(SparkBatchDescriptor batch)
        {
            var descriptors = new List<SparkViewDescriptor>();
            foreach (var entry in batch.Entries)
                descriptors.AddRange(CreateDescriptors(entry));
            return descriptors;
        }

        public IList<SparkViewDescriptor> CreateDescriptors(SparkBatchEntry entry)
        {
            var descriptors = new List<SparkViewDescriptor>();

            var controllerName = RemoveSuffix(entry.ControllerType.Name, "Controller");

            var viewNames = new List<string>();
            var includeViews = entry.IncludeViews;
            if (includeViews.Count == 0)
                includeViews = new[] { "*" };

            foreach (var include in includeViews)
            {
                if (include.EndsWith("*"))
                {
                    foreach (var fileName in ViewFolder.ListViews(controllerName))
                    {
                        if (!string.Equals(Path.GetExtension(fileName), ".spark", StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        var potentialMatch = Path.GetFileNameWithoutExtension(fileName);
                        if (!TestMatch(potentialMatch, include))
                            continue;

                        var isExcluded = false;
                        foreach (var exclude in entry.ExcludeViews)
                        {
                            if (!TestMatch(potentialMatch, RemoveSuffix(exclude, ".spark")))
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
                    viewNames.Add(RemoveSuffix(include, ".spark"));
                }
            }

            foreach (var viewName in viewNames)
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
            var descriptor = DescriptorBuilder.BuildDescriptor(
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


        private static string RemoveSuffix(string value, string suffix)
        {
            if (value.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
                return value.Substring(0, value.Length - suffix.Length);
            return value;
        }

        private static bool TestMatch(string potentialMatch, string pattern)
        {
            if (!pattern.EndsWith("*"))
            {
                return string.Equals(potentialMatch, pattern, StringComparison.InvariantCultureIgnoreCase);
            }

            // raw wildcard matches anything that's not a partial
            if (pattern == "*")
            {
                return !potentialMatch.StartsWith("_");
            }

            // otherwise the only thing that's supported is "starts with"
            return potentialMatch.StartsWith(pattern.Substring(0, pattern.Length - 1),StringComparison.InvariantCultureIgnoreCase);
        }

    }
}