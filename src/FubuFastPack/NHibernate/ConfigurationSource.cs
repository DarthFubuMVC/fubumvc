using System;
using System.Collections.Generic;
using FluentNHibernate;
using NHibernate.Cfg;

namespace FubuFastPack.NHibernate
{
    public class ConfigurationSource : IConfigurationSource
    {
        private readonly Lazy<Configuration> _configuration;
        private readonly Lazy<PersistenceModel> _model;
        private readonly IEnumerable<IConfigurationModifier> _registries;

        public ConfigurationSource(IEnumerable<IConfigurationModifier> registries)
        {
            _registries = registries;

            _model = new Lazy<PersistenceModel>(() =>
            {
                var model = new PersistenceModel();
                _registries.Each(r => r.Configure(model));

                return model;
            });

            _configuration = new Lazy<Configuration>(() =>
            {
                var configuration = new Configuration();
                _registries.Each(r => r.ApplyProperties(configuration));

                _model.Value.Configure(configuration);

                _registries.Each(r => r.Configure(configuration));

                return configuration;
            });
        }

        public Configuration Configuration()
        {
            return _configuration.Value;
        }

        public PersistenceModel Model()
        {
            return _model.Value;
        }



    }


}