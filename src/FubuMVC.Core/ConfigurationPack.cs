using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core
{
    public class ConfigurationPack
    {
        private readonly IList<Action<ConfigurationGraph>> _actions = new List<Action<ConfigurationGraph>>();
        private string _configurationType;

        private Action<ConfigurationGraph> alter
        {
            set { _actions.Add(value); }
        }

        public void For(string configurationType)
        {
            _configurationType = configurationType;
        }

        public void Add(IConfigurationAction action)
        {
            alter = graph => graph.AddConfiguration(action, _configurationType);
        }

        public void Add<T>() where T : IConfigurationAction, new()
        {
            var type = _configurationType;
            alter = graph => graph.AddConfiguration(new T(), type);
        }

        public void WriteTo(ConfigurationGraph configurationGraph)
        {
            _actions.Each(x => x(configurationGraph));
        }
    }
}