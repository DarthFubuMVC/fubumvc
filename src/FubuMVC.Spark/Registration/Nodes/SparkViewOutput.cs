using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark.Registration.Nodes
{
    public class SparkViewOutput : OutputNode<RenderSparkBehavior>
    {
        private static readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly SparkItem _item;

        static SparkViewOutput()
        {
            _cache = new Dictionary<int, ISparkViewEntry>();
        }

        public SparkViewOutput(SparkItem item)
        {
            _item = item;
        }

        protected override void configureObject(ObjectDef def)
        {
            var renderer = def.DependencyByType(typeof (ISparkViewRenderer), typeof (SparkViewRenderer));
            var strategies = renderer.EnumerableDependenciesOf<IRenderStrategy>();

            configureStrategy<NestedRenderStrategy, NestedRenderAction>(strategies, createDescriptor(false));
            configureStrategy<AjaxRenderStrategy, DefaultRenderAction>(strategies, createDescriptor(false));
            configureStrategy<DefaultRenderStrategy, DefaultRenderAction>(strategies, createDescriptor(true));
        }

        private SparkViewDescriptor createDescriptor(bool useMaster)
        {
            var descriptor = new SparkViewDescriptor().AddTemplate(_item.ViewPath);
            if(useMaster && _item.Master != null)
            {
                descriptor.AddTemplate(_item.Master.ViewPath);
            }
            return descriptor;
        }

        private static void configureStrategy<TStrategy, TViewRenderer>(ListDependency strategies, SparkViewDescriptor descriptor)
            where TStrategy : IRenderStrategy
            where TViewRenderer : IRenderAction
        {
            var factory = new ObjectDef { Type = typeof(ViewFactory) };
            var engine = factory.DependencyByType(typeof(IViewEntrySource), typeof(ViewEntrySource));
            engine.DependencyByValue(_cache);
            engine.DependencyByValue(descriptor);


            strategies.AddType(typeof (TStrategy))
                .DependencyByType(typeof (IRenderAction), typeof (TViewRenderer))
                .DependencyByType<IViewFactory>(factory);
        }

        public override string Description
        {
            get { return string.Format("Spark [{0}]", _item.RelativePath()); }
        }
    }
}