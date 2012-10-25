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
        private readonly IList<ActionLog> _sources = new List<ActionLog>();
        private ActionLog _currentSource;

        public ConfigLog(BehaviorGraph graph)
        {
            _graph = graph;
        }

        internal void Import(ConfigLog log)
        {
            _sources.AddRange(log._sources);
        }

        public IEnumerable<T> EventsOfType<T>()
        {
            return _sources.SelectMany(x => x.Events.OfType<T>());
        }

        public void RunAction(FubuRegistry provenance, IConfigurationAction action)
        {
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

        public void RecordEvents(BehaviorChain chain, ITracedModel model)
        {
            model.RecordEvents(e =>
            {
                _currentSource.AddEvent(e);
            });
        }        

        public IEnumerable<ActionLog> AllConfigSources()
        {
            return _sources;
        }

        public ActionLog SourceFor(Guid id)
        {
            return _sources.FirstOrDefault(x => x.Id == id);
        }
    }
}