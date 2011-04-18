using System.Collections.Generic;
using FubuCore.Util;

namespace Bottles.Deployment
{
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
}