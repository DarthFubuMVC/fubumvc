using System;
using System.Collections;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration.Diagnostics
{
    // TODO -- most of this is going to get flushed out during diagnostics
    public class ConfigLog
    {
        private readonly IList<ConfigGraph> _allGraphs = new List<ConfigGraph>();

        public ConfigLog()
        {
            // TODO -- this will have to account for services too
            _uniqueChains = new Lazy<IEnumerable<ProvenanceChain>>(() => {
                return _allGraphs.SelectMany(x => x.AllLogs()).Select(x => x.ProvenanceChain).Distinct();
            });
        }

        internal void Import(ConfigGraph graph)
        {
            _allGraphs.Add(graph);
        }

        internal void Import(ConfigGraph graph, IEnumerable<Provenance> forebears)
        {
            graph.PrependProvenance(forebears);
            _allGraphs.Add(graph);
        }

        public IEnumerable<T> EventsOfType<T>()
        {
            return _allGraphs.SelectMany(x => x.AllEvents<T>());
        }

        public IEnumerable<ActionLog> AllLogs()
        {
            throw new NotImplementedException();
            //return _sources;
        }

        public ActionLog SourceFor(Guid id)
        {
            throw new NotImplementedException();
            //return _sources.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<ActionLog> LogsForBottle(string bottleName)
        {
            throw new NotImplementedException();
        } 

        public IEnumerable<ActionLog> LogsForProvenance(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ActionLog> LogsForProvenance(Provenance provenance)
        {
            throw new NotImplementedException();
        }

        private readonly Lazy<IEnumerable<ProvenanceChain>> _uniqueChains; 

        public IEnumerable<ProvenanceChain> UniqueProvenanceChains()
        {
            return _uniqueChains.Value;
        }
    }
}