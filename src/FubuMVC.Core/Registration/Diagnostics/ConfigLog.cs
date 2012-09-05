using System;
using System.Collections;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class ConfigLog
    {
        private readonly BehaviorGraph _graph;
        private readonly Cache<object, IList<NodeEvent>> _bySubject = new Cache<object, IList<NodeEvent>>(o => new List<NodeEvent>());
        private readonly IList<ConfigSource> _sources = new List<ConfigSource>();
        private ConfigSource _currentSource;

        public ConfigLog(BehaviorGraph graph)
        {
            _graph = graph;
        }

        public void RunAction(IConfigurationAction action)
        {
            StartSource(action);

            action.Configure(_graph);

            _graph.Behaviors.Each(chain =>
            {
                TracedModelsFor(chain).Each(node => RecordEvents(chain, node));
            });
        }

        public IEnumerable<ITracedModel> TracedModelsFor(BehaviorChain chain)
        {
            yield return chain;

            if (chain.Route != null)
            {
                yield return (ITracedModel) chain.Route;
            }

            foreach (var node in chain)
            {
                yield return node;
            }
        }

        public ConfigSource StartSource(IConfigurationAction action)
        {
            var source = new ConfigSource(action);
            _sources.Add(source);

            _currentSource = source;

            return _currentSource;
        }

        public void RecordEvents(BehaviorChain chain, ITracedModel model)
        {
            model.RecordEvents(e =>
            {
                e.Chain = chain;
                _currentSource.AddEvent(e);
                _bySubject[model].Add(e);
            });
        }        


        public IEnumerable<NodeEvent> EventsBySubject(object subject)
        {
            return _bySubject[subject];
        }

        public IEnumerable<ConfigSource> AllConfigSources()
        {
            return _sources;
        }

        public ConfigSource SourceFor(Guid id)
        {
            return _sources.FirstOrDefault(x => x.Id == id);
        }
    }
}