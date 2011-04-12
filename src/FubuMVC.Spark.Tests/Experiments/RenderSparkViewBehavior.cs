using System.IO;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Spark.Tests.Experiments
{
    public class RenderSparkViewBehavior : IActionBehavior
    {
        private readonly SparkViewOutput _output;

        public RenderSparkViewBehavior(SparkViewOutput output)
        {
            _output = output;
        }

        public void Invoke()
        {
            renderStream(_output.Render());
        }

        public void InvokePartial()
        {
            renderStream(_output.RenderPartial());
        }

        private void renderStream(Stream stream)
        {
            //TBD
        }

    }
}