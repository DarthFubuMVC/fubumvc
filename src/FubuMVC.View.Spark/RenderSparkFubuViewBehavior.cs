using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

namespace FubuMVC.View.Spark
{
    public class RenderSparkFubuViewBehavior : RenderFubuViewBehavior
    {
        public RenderSparkFubuViewBehavior(SparkViewEngine<IFubuView> engine, IFubuRequest request, ViewPath view, IViewActivator activator) : base(engine, request, view, activator) { }
    }
}