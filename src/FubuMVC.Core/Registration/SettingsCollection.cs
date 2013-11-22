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
        private readonly SettingsCollection _parent;
        private readonly Cache<Type, object> _settings = new Cache<Type, object>();

        public SettingsCollection(SettingsCollection parent)
        {
            _settings.OnMissing = buildDefault;
            _parent = parent;
        }

        private static object buildDefault(Type type)
        {
            if (type.IsValueType)
            {
                return typeof(DefaultMaker<>).CloseAndBuildAs<IDefaultMaker>(type).Default();
            }

            if (type.IsConcreteWithDefaultCtor())
            {
                var provider = new AppSettingsProvider(ObjectResolver.Basic());
                return provider.SettingsFor(type);
            }

            throw new ArgumentOutOfRangeException("Can only build default values for concrete classes with a default constructor and value types");
        }

        public T Get<T>()
        {
            if (_parent != null && !HasExplicit<T>() && (_parent._settings.Has(typeof(T)) || typeof(T).HasAttribute<ApplicationLevelAttribute>()))
            {
                return (T)_parent._settings[typeof(T)];
            }

            return (T) _settings[typeof (T)];
        }

        public void Alter<T>(Action<T> alteration)
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

        public void Replace<T>(T settings)
        {
            _settings[typeof (T)] = settings;
        }

        public bool HasExplicit<T>()
        {
            return _settings.Has(typeof (T));
        }

        public class DefaultMaker<T> : IDefaultMaker
        {
            public object Default()
            {
                return default(T);
            }
        }

        public interface IDefaultMaker
        {
            object Default();
        }

        public void ForAllSettings(Action<Type, object> callback)
        {
            _settings.Each(callback);
        }
    }
}