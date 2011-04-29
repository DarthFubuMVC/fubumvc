using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.Rendering
{
    public interface ISparkViewRenderer
    {
        void Render();
    }

    public class SparkViewRenderer : ISparkViewRenderer
    {
        private readonly IEnumerable<IRenderStrategy> _strategies;
        public SparkViewRenderer(IEnumerable<IRenderStrategy> strategies)
        {
            _strategies = strategies;
        }

        public void Render()
        {
            _strategies.First(x => x.Applies()).Invoke();
        }
    }
}