using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;

namespace FubuMVC.View.Spark
{
    public class SparkViewNode : OutputNode
    {
        private readonly string _viewPath;

        public SparkViewNode(string viewPath) : base(typeof(RenderSparkFubuViewBehavior))
        {
            _viewPath = viewPath;
        }

        public override string Description
        {
            get
            {
                return "Render Spark View: " + _viewPath;
            }
        }

        protected override void configureObject(ObjectDef def)
        {
            def.Child(new ViewPath
            {
                ViewName = _viewPath
            });
        }

    }
}