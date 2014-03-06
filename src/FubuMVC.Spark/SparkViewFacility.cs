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

        public Task<IEnumerable<IViewToken>> FindViews(BehaviorGraph graph)
        {
            return Task.Factory.StartNew(() => findViews(graph));
        }

        private IEnumerable<IViewToken> findViews(BehaviorGraph graph)
        {
            var sparkSettings = graph.Settings.Get<SparkEngineSettings>();

            var composer = new TemplateComposer<ISparkTemplate>();
            sparkSettings.Configure(composer);

            var templates = graph.Files.FindFiles(sparkSettings.Search)
                .Select(file => {
                    var template = new SparkTemplate(file);
                    template.Descriptor = new SparkDescriptor(template, _engine);


                    composer.Compose(template);

                    return template;
                });

            return templates.Select(x => new SparkViewToken((SparkDescriptor) x.Descriptor));
        }
    }
}