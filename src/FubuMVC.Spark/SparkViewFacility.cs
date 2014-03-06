using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
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

        public Task<IEnumerable<ITemplateFile>> FindViews(BehaviorGraph graph)
        {
            return Task.Factory.StartNew(() => findViews(graph));
        }

        private IEnumerable<ITemplateFile> findViews(BehaviorGraph graph)
        {
            var sparkSettings = graph.Settings.Get<SparkEngineSettings>();


            return graph.Files.FindFiles(sparkSettings.Search)
                .Select(file => new SparkTemplate(file, _engine));
        }
    }
}