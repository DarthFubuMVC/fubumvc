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
        internal void Import(ConfigGraph graph)
        {
            throw new NotImplementedException();
        }

        internal void Import(ConfigGraph graph, IEnumerable<Provenance> forebears)
        {
            throw new NotImplementedException();
            //graph.
        }

        public IEnumerable<T> EventsOfType<T>()
        {
            throw new NotImplementedException();
            //return _sources.SelectMany(x => x.Events.OfType<T>());
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
    }
}