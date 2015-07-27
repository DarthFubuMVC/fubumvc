using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using StructureMap;

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

        public Task<Task[]> ApplyAll(SettingsCollection settings, FubuRegistry registry)
        {
            return _types.ContinueWith(t =>
            {
                return t.Result.Select(type =>
                {
                    var feature = typeof (Feature<>).CloseAndBuildAs<IFeature>(type);
                    return feature.Apply(settings, registry);
                }).ToArray();
            });
        }

        public interface IFeature
        {
            Task Apply(SettingsCollection settings, FubuRegistry registry);
        }

        public class Feature<T> : IFeature where T : IFeatureSettings
        {
            public Task Apply(SettingsCollection settings, FubuRegistry registry)
            {
                return settings.GetTask<T>().ContinueWith(t => t.Result.Apply(registry));
            }
        }


    }
}