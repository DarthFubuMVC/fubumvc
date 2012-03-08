using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class ConfigLog
    {
        private readonly Cache<object, IList<NodeEvent>> _bySubject = new Cache<object, IList<NodeEvent>>(o => new List<NodeEvent>());
        private readonly IList<ConfigSource> _sources = new List<ConfigSource>();
        private ConfigSource _currentSource;

        public ConfigSource StartSource(IConfigurationAction action)
        {
            var source = new ConfigSource(action);
            _sources.Add(source);

            _currentSource = source;

            return _currentSource;
        }

        public void RecordEvents(ITracedModel model)
        {
            model.RecordEvents(e =>
            {
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