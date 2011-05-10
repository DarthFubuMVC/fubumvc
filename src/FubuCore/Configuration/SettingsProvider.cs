using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;

namespace FubuCore.Configuration
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly IObjectResolver _resolver;
        private readonly IEnumerable<ISettingsSource> _sources;

        public SettingsProvider(IObjectResolver resolver, IEnumerable<SettingsData> settings)
            : this(resolver, new ISettingsSource[]{new SettingsSource(settings)})
        {
            
        }

        public SettingsProvider(IObjectResolver resolver, IEnumerable<ISettingsSource> sources)
        {
            _resolver = resolver;
            _sources = sources;
        }

        public T SettingsFor<T>() where T : class, new()
        {
            return (T) SettingsFor(typeof (T));
        }

        public object SettingsFor(Type settingsType)
        {
            SettingsRequestData settingsData = getSettingsData();
            var prefixedData = new PrefixedRequestData(settingsData, settingsType.Name + ".");

            var result = _resolver.BindModel(settingsType, prefixedData);
            result.AssertNoProblems(settingsType);

            return result.Value;
        }

        private SettingsRequestData getSettingsData()
        {
            return new SettingsRequestData(_sources.SelectMany(x => x.FindSettingData()));
        }

        public IEnumerable<SettingDataSource> CreateDiagnosticReport()
        {
            return getSettingsData().CreateDiagnosticReport();
        }

        public static SettingsProvider For(params SettingsData[] data)
        {
            return new SettingsProvider(ObjectResolver.Basic(), data);
        }
    }

}