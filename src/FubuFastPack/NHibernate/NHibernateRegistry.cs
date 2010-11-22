using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using FluentNHibernate;
using FluentNHibernate.Conventions;
using FubuCore.Util;
using NHibernate.Cfg;
using NHibernate.Event;

namespace FubuFastPack.NHibernate
{
    public class NHibernateRegistry : IConfigurationModifier
    {
        private readonly IList<Action<Configuration>> _configurationModifications = new List<Action<Configuration>>();
        private readonly IList<Action<PersistenceModel>> _modelModifications = new List<Action<PersistenceModel>>();
        private readonly Cache<string, string> _properties = new Cache<string, string>();

        private Action<PersistenceModel> modifyModel
        {
            set { _modelModifications.Add(value); }
        }

        private Action<Configuration> modifyConfiguration
        {
            set { _configurationModifications.Add(value); }
        }

        void IConfigurationModifier.ApplyProperties(Configuration configuration)
        {
            _properties.Each((key, value) => configuration.SetProperty(key, value));
        }

        void IConfigurationModifier.Configure(PersistenceModel model)
        {
            _modelModifications.Each(m => m(model));
        }

        void IConfigurationModifier.Configure(Configuration configuration)
        {
            _configurationModifications.Each(x => x(configuration));
        }

        // TODO -- want this to be done in combination
        public void SetProperties(IDictionary<string, string> props)
        {
            props.Each(p => _properties[p.Key] = p.Value);
        }

        public void Configure(Action<Configuration> configure)
        {
            modifyConfiguration = configure;
        }

        public void MappingsFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            modifyModel = m => assemblies.Each(a => m.AddMappingsFromAssembly(a));
        }

        public void MappingsFromAssembly(Assembly assembly)
        {
            modifyModel = m => m.AddMappingsFromAssembly(assembly);
        }

        public void MappingFromThisAssembly()
        {
            modifyModel = m => m.AddMappingsFromAssembly(findTheCallingAssembly());
        }

        private static Assembly findTheCallingAssembly()
        {
            var trace = new StackTrace(Thread.CurrentThread, false);

            var thisAssembly = Assembly.GetExecutingAssembly();
            Assembly callingAssembly = null;
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }

        public void Convention<T>() where T : IConvention, new()
        {
            modifyModel = m => m.Conventions.Add<T>();
        }

        public void PreInsertEventListener<T>() where T : IPreInsertEventListener, new()
        {
            PreInsertEventListener(new T());
        }

        public void PreInsertEventListener(IPreInsertEventListener listener)
        {
            modifyConfiguration = c => c.AddPreInsertListener(listener);
        }

        public void PreUpdateEventListener<T>() where T : IPreUpdateEventListener, new()
        {
            PreUpdateEventListener(new T());
        }

        public void PreUpdateEventListener(IPreUpdateEventListener listener)
        {
            modifyConfiguration = c => c.AddPreUpdateListener(listener);
        }
    }
}