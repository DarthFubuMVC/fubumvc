using System;
using System.Collections.Generic;
using System.Threading;
using Spark.FileSystem;
using Spark.Web.FubuMVC.ViewLocation;

namespace Spark.Web.FubuMVC
{
    public class SparkViewFactory
    {
        private IDescriptorBuilder _descriptorBuilder;
        private ISparkViewEngine _engine;

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
                _engine.DefaultPageBaseType = typeof (SparkView).FullName;
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
    }
}