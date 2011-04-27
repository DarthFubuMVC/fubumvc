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
            var strategy = _strategies.Where(x => x.Applies()).First();
            strategy.Invoke();
        }
    }
}