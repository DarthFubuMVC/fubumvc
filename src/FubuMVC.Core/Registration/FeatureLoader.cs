using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore;
using StructureMap.Graph.Scanning;

namespace FubuMVC.Core.Registration
{
    public class FeatureLoader
    {
        private Task<IEnumerable<Type>> _types;

        public void LookForFeatures()
        {
            _types = TypeRepository
                .FindTypes(
                    GetType().Assembly,
                    TypeClassification.Concretes | TypeClassification.Closed,
                    x => x.CanBeCastTo<IFeatureSettings>()
                );
        }

        public Task ApplyAll(SettingsCollection settings, FubuRegistry registry)
        {
            return
                _types.ContinueWith(
                    t => { t.Result.Each(type => settings.Get(type).As<IFeatureSettings>().Apply(registry)); });
        }

        public interface IFeature
        {
            void Apply(SettingsCollection settings, FubuRegistry registry);
        }

        public class Feature<T> : IFeature where T : class, IFeatureSettings
        {
            public void Apply(SettingsCollection settings, FubuRegistry registry)
            {
                settings.Get<T>().As<IFeatureSettings>().Apply(registry);
            }
        }
    }
}