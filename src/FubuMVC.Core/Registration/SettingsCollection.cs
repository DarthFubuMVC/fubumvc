using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Configuration;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.ScheduledJobs;

namespace FubuMVC.Core.Registration
{
    public class SettingsCollection
    {
        public static readonly Lazy<ISettingsProvider> SettingsProvider =
            new Lazy<ISettingsProvider>(() => new AppSettingsProvider(ObjectResolver.Basic()));

        private readonly Cache<Type, object> _settings = new Cache<Type, object>();

        public SettingsCollection()
        {
            _settings.OnMissing = buildDefault;

            warmUp<ChannelGraph>();
            warmUp<PollingJobSettings>();
            warmUp<ScheduledJobGraph>();
            warmUp<TransportSettings>();
        }

        private void warmUp<T>() where T : class
        {
            Alter<T>(x => { });
        }

        private static object buildDefault(Type type)
        {
            return SettingsProvider.Value.SettingsFor(type);
        }

        public T Get<T>() where T : class
        {
            return _settings[typeof (T)].As<T>();
        }


        public void Alter<T>(Action<T> alteration) where T : class
        {
            alteration(Get<T>());
        }

        public void Replace<T>(T settings) where T : class
        {
            _settings[typeof (T)] = settings;
        }


        public bool HasExplicit<T>()
        {
            return _settings.Has(typeof (T));
        }

        public void Register(ServiceRegistry registry)
        {
            _settings.Each((t, o) =>
            {
                var registrar = typeof (Registrar<>).CloseAndBuildAs<IRegistrar>(o, t);
                registrar.Register(registry);
            });
        }

        public interface IRegistrar
        {
            void Register(ServiceRegistry registry);
        }

        public class Registrar<T> : IRegistrar where T : class
        {
            private readonly T _settings;

            public Registrar(T settings)
            {
                _settings = settings;
            }

            public void Register(ServiceRegistry registry)
            {
                registry.For<T>().ClearAll().Use(_settings);
            }
        }

        public object Get(Type type)
        {
            return _settings[type];
        }
    }


    public static class TaskExtensions
    {
        public static T Result<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
    }
}