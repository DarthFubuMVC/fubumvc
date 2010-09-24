using System;
using StructureMap;
using FubuMVC.Core;
using Spark.Web.FubuMVC.Extensions;
using Spark.Web.FubuMVC.ViewLocation;
using System.Collections.Generic;
using Spark.FileSystem;

namespace Spark.Web.FubuMVC.Bootstrap
{
    public class SparkDefaultStructureMapRegistry : FubuRegistry
    {
        private SparkViewFactory _sparkViewFactory;
        public SparkDefaultStructureMapRegistry(bool debuggingEnabled, string controllerAssembly)
        {
            _sparkViewFactory = ObjectFactory.Container.GetInstance<SparkViewFactory>();

            IncludeDiagnostics(debuggingEnabled);

            Applies.ToAssembly(controllerAssembly);

            Routes.IgnoreControllerNamespaceEntirely();
        }

        protected void AttachViewsBy(Func<Type, bool> actionNameFilter, Func<string, string> actionNameConvention)
        {
            Views.Facility(new SparkViewFacility(_sparkViewFactory, actionNameFilter, actionNameConvention))
                            .TryToAttach(x => x.BySparkViewDescriptors(actionNameConvention));
        }

        protected void AddViewFolder(string virtualFolderRoot)
        {
            _sparkViewFactory = ObjectFactory.Container.GetInstance<SparkViewFactory>();
            ((SparkSettings)_sparkViewFactory.Settings)
                .AddViewFolder(ViewFolderType.VirtualPathProvider,
                new Dictionary<string, string> { { "virtualBaseDir", virtualFolderRoot } });
        }

    }
}