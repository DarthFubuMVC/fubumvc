using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Binding;
using FubuCore.Configuration;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;

namespace Bottles.Deployment
{
    public class HostManifest
    {
        private static readonly IObjectResolver _resolver = ObjectResolver.Basic();

        private readonly IList<BottleReference> _bottles = new List<BottleReference>();
        private readonly IList<ISettingsData> _data = new List<ISettingsData>();

        public HostManifest(string name)
        {
            Name = name;
        }

        public T GetDirective<T>() where T : class, new()
        {
            var provider = new SettingsProvider(_resolver, _data);
            return provider.SettingsFor<T>();
        }

        public IDirective GetDirective(Type directiveType)
        {
            var provider = new SettingsProvider(_resolver, _data);
            return (IDirective) provider.SettingsFor(directiveType);
        }

        public string Name { get; private set; }
    
        public void RegisterBottle(BottleReference reference)
        {
            _bottles.Add(reference);
        }

        public IEnumerable<BottleReference> BottleReferences
        {
            get { return _bottles; }
        }

        public void RegisterSettings(ISettingsData data)
        {
            _data.Add(data);
        }

        public void RegisterValue<T>(Expression<Func<T, object>> expression, object value) where T : IDirective
        {
            var key = "{0}.{1}".ToFormat(typeof (T).Name, expression.ToAccessor().PropertyNames.Join("."));
            var data = new InMemorySettingsData(SettingCategory.core).With(key, value.ToString());
            
            RegisterSettings(data);
        }

        public void Append(HostManifest otherHost)
        {
            _bottles.Fill(otherHost._bottles);
            _data.AddRange(otherHost._data);
        }

        public void Prepend(HostManifest otherHost)
        {
            _bottles.Fill(otherHost._bottles);
            var ours = _data.ToArray();
            _data.Clear();
            _data.AddRange(otherHost._data);
            _data.AddRange(ours);
        }

        /// <summary>
        /// This is only used for testing
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISettingsData> AllSettingsData()
        {
            return _data;
        }

        public void RegisterBottles(IEnumerable<BottleReference> references)
        {
            _bottles.AddRange(references);
        }


    }
}