using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Configuration
{
    public class ConfigurationPack
    {
        private readonly IList<Action<ConfigGraph>> _actions = new List<Action<ConfigGraph>>();
        private string _configurationType;

        private Action<ConfigGraph> alter
        {
            set { _actions.Add(value); }
        }

        public void For(string configurationType)
        {
            _configurationType = configurationType;
        }

        public void Add(IConfigurationAction action)
        {
            alter = graph => graph.Add(action, _configurationType);
        }

        public void Add<T>() where T : IConfigurationAction, new()
        {
            var type = _configurationType;
            alter = graph => graph.Add(new T(), type);
        }

        public void Services<T>() where T : ServiceRegistry, new()
        {
            Services(new T());
        }

        public void Services(ServiceRegistry registry)
        {
            alter = graph => graph.Add(registry);
        }

        public void WriteTo(ConfigGraph ConfigGraph)
        {
            _actions.Each(x => x(ConfigGraph));
        }
    }
}