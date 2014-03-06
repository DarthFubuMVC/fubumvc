using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly SparkViewEngine _engine;

        public SparkViewFacility(SparkViewEngine engine)
        {
            _engine = engine;
        }

        public Task<IEnumerable<IViewToken>> FindViews(BehaviorGraph graph)
        {
            return Task.Factory.StartNew(() => findViews(graph));
        }

        private IEnumerable<IViewToken> findViews(BehaviorGraph graph)
        {
            var sparkSettings = graph.Settings.Get<SparkEngineSettings>();


            return graph.Files.FindFiles(sparkSettings.Search)
                .Select(file => {
                    var template = new SparkTemplate(file);
                    return new SparkViewToken(template, _engine);
                });
        }
    }
}