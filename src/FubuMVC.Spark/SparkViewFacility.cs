using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly SparkTemplateRegistry _templateRegistry;
        private readonly Parsings _parsings;

        public SparkViewFacility(SparkTemplateRegistry templateRegistry, Parsings parsings)
        {
            _templateRegistry = templateRegistry;
            _parsings = parsings;
        }

        public IEnumerable<IViewToken> FindViews(BehaviorGraph graph)
        {
            var sparkSettings = graph.Settings.Get<SparkEngineSettings>();
            RegisterTemplates(graph.Files, sparkSettings);
            ComposeTemplates(sparkSettings);

            return FindTokens();
        }

        public IEnumerable<IViewToken> FindTokens()
        {            
            return _templateRegistry.DescriptorsWithViewModels<SparkDescriptor>()
                .Select(x => new SparkViewToken(x));
        }

        public void RegisterTemplates(IFubuApplicationFiles fubuFiles, SparkEngineSettings settings)
        {
            fubuFiles.FindFiles(settings.Search).Each(file => 
                _templateRegistry.Register(new Template(file.Path, file.ProvenancePath, file.Provenance)));            
        }

        public void ComposeTemplates(SparkEngineSettings settings)
        {
            _templateRegistry.Each(_parsings.Process);         
            var composer = new TemplateComposer<ITemplate>(_parsings);   
            settings.Configure(composer);
            composer.Compose(_templateRegistry);
        }
    }
}