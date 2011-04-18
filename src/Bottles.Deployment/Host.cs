using System;
using System.Collections.Generic;
using FubuCore.Configuration;
using FubuCore.Util;

namespace Bottles.Deployment
{
    public interface IHostManifest
    {
        T GetSettings<T>();
        string Name { get; }
    }

    public class Recipe
    {
        private readonly string _name;
        private readonly Cache<string, IHostManifest> _hosts = new Cache<string, IHostManifest>(name => new HostManifest(name));
        private readonly List<string> _dependencies = new List<string>();

        public Recipe(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public IHostManifest HostFor(string name)
        {
            return _hosts[name];
        }

        public IEnumerable<IHostManifest> Hosts
        {
            get
            {
                return _hosts.GetAll();
            }
        }

        public void RegisterDependency(string recipeName)
        {
            _dependencies.Add(recipeName);
        }

        public void RegisterHost(IHostManifest host)
        {
            _hosts[host.Name] = host;
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