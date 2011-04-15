using System;
using System.Collections.Generic;
using FubuCore.Configuration;
using FubuCore.Util;

namespace Bottles.Deployment
{
    public interface IHostManifest
    {
        T GetSettings<T>();
    }

    public class Recipe
    {
        private readonly Cache<string, HostManifest> _hosts = new Cache<string, HostManifest>(name => new HostManifest(name));
        private readonly List<string> _dependencies = new List<string>();

        public HostManifest HostFor(string name)
        {
            return _hosts[name];
        }

        public void RegisterDependency(string recipeName)
        {
            _dependencies.Add(recipeName);
        }
    }

    public class HostManifest : IHostManifest
    {
        private readonly IList<BottleReference> _bottles = new List<BottleReference>();
        private readonly IList<ISettingsData> _data = new List<ISettingsData>();

        public HostManifest(string name)
        {
            Name = name;
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

        public void Append(HostManifest manifest)
        {
            throw new NotImplementedException();
        }

        public T GetSettings<T>()
        {
            throw new NotImplementedException();
        }
    }
}