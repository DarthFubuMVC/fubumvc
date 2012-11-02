using System;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core
{
    public class BehaviorGraphBuilder
    {
        public BehaviorGraph BuildForImport(FubuRegistry registry)
        {
            var types = registry.BuildTypePool();
        
            throw new NotImplementedException();
        }

        public BehaviorGraph Build(FubuRegistry registry)
        {
            var types = registry.BuildTypePool();

            // Need to register the ConfigGraph!!!!

            // use SystemServicesPack
            // use DefaultConfigurationPack

            throw new NotImplementedException();
        }

        /*
         * 
         * graph.Services.AddService(this); <----- ConfigLog
         * 
         * 
        public BehaviorGraph BuildForImport(BehaviorGraph parent)
        {
            IEnumerable<IConfigurationAction> lightweightActions = _configurations[ConfigurationType.Settings]
                .Union(_configurations[ConfigurationType.Discovery])
                //.Union(_imports)
                .Union(_configurations[ConfigurationType.Explicit])
                .Union(_configurations[ConfigurationType.Policy])
                .Union(_configurations[ConfigurationType.Reordering]);

            BehaviorGraph graph = BehaviorGraph.ForChild(parent);

            lightweightActions.Each(x => graph.Log.RunAction(_registry, x));

            return graph;
        }
         * 
         * 
         * 
         * 
        public IEnumerable<IConfigurationAction> AllConfigurationActions()
        {
            return _configurations[ConfigurationType.Settings]
                .Union(_configurations[ConfigurationType.Discovery])
                //.Union(UniqueImports())
                .Union(_configurations[ConfigurationType.Explicit])
                .Union(_configurations[ConfigurationType.Policy])
                .Union(_configurations[ConfigurationType.Navigation])
                .Union(_configurations[ConfigurationType.ByNavigation])
                .Union(_configurations[ConfigurationType.Attributes])
                .Union(_configurations[ConfigurationType.ModifyRoutes])
                .Union(_configurations[ConfigurationType.InjectNodes])
                .Union(_configurations[ConfigurationType.Conneg])
                .Union(_configurations[ConfigurationType.Attachment])

                .Union(_configurations[ConfigurationType.Reordering])
                .Union(_configurations[ConfigurationType.Instrumentation]);
        }
         */
    }
}