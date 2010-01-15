using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;

namespace FubuMVC.View.Spark
{
    public class SparkViewNode : OutputNode
    {
        private readonly string _name;

        public SparkViewNode(string name) : base(typeof(RenderSparkFubuViewBehavior))
        {
            _name = name;
        }

        public override string Description
        {
            get
            {
                return "Render Spark View: " + _name;
            }
        }

        protected override void configureObject(ObjectDef def)
        {
            def.Child(new ViewPath
            {
                ViewName = _name
            });
        }

    }
}