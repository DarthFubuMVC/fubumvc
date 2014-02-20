using System;
using FubuCore.Binding;
using FubuCore.Configuration;
using FubuCore.Util;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration
{
    public class SettingsCollection
    {
        private readonly static Lazy<ISettingsProvider> SettingsProvider = new Lazy<ISettingsProvider>(() => new AppSettingsProvider(ObjectResolver.Basic()));

        private readonly SettingsCollection _parent;
        private readonly Cache<Type, object> _settings = new Cache<Type, object>();

        public SettingsCollection(SettingsCollection parent)
        {
            _settings.OnMissing = buildDefault;
            _parent = parent;
        }

        private static object buildDefault(Type type)
        {
            var templateType = type.IsConcreteWithDefaultCtor()
                ? typeof (AppSettingMaker<>)
                : typeof (DefaultMaker<>);

            return templateType.CloseAndBuildAs<IDefaultMaker>(type).MakeDefault();
        }

        public T Get<T>() where T : class
        {
            if (_parent != null && !HasExplicit<T>() && (_parent._settings.Has(typeof(T)) || typeof(T).HasAttribute<ApplicationLevelAttribute>()))
            {
                return (T)_parent._settings[typeof(T)];
            }

            return (T) _settings[typeof (T)];
        }

        public void Alter<T>(Action<T> alteration) where T : class
        {
            if (_parent != null && typeof(T).HasAttribute<ApplicationLevelAttribute>())
            {
                alteration(_parent.Get<T>());
            }
            else
            {
                alteration((T)_settings[typeof(T)]);
            }
        }

        public void Replace<T>(T settings) where T : class
        {
            _settings[typeof (T)] = settings;
        }

        public bool HasExplicit<T>() 
        {
            return _settings.Has(typeof (T));
        }

        public void ForAllSettings(Action<Type, object> callback)
        {
            _settings.Each(callback);
        }

        public interface IDefaultMaker
        {
            object MakeDefault();
        }

        public class DefaultMaker<T> : IDefaultMaker
        {
            public object MakeDefault()
            {
                return default(T);
            }
        }

        public class AppSettingMaker<T> : IDefaultMaker where T : class, new()
        {
            public object MakeDefault()
            {
                return SettingsProvider.Value.SettingsFor<T>();
            }
        }
    }





}