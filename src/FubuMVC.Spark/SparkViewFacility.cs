using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly SparkTemplateRegistry _templateRegistry;
        private readonly Parsings _parsings;
        private readonly SparkViewEngine _engine;

        public SparkViewFacility(SparkTemplateRegistry templateRegistry, Parsings parsings, SparkViewEngine engine)
        {
            _templateRegistry = templateRegistry;
            _parsings = parsings;
            _engine = engine;
        }

        public Task<IEnumerable<IViewToken>> FindViews(BehaviorGraph graph)
        {
            return Task.Factory.StartNew(() => findViews(graph));
        }



        private IEnumerable<IViewToken> findViews(BehaviorGraph graph)
        {
            var sparkSettings = graph.Settings.Get<SparkEngineSettings>();

            _templateRegistry.Each(_parsings.Process);         
            var composer = new TemplateComposer<ISparkTemplate>(_parsings); 
            sparkSettings.Configure(composer);

            var templates = graph.Files.FindFiles(sparkSettings.Search)
                .Select(file => {
                    var template = new SparkTemplate(file.Path, file.ProvenancePath, file.Provenance);
                    template.Descriptor = new SparkDescriptor(template, _engine);


                    composer.Compose(template);

                    return template;
                });

            return templates.Select(x => new SparkViewToken((SparkDescriptor) x.Descriptor));
        } 


    }
}