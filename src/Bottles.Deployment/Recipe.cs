using System.Collections.Generic;
using FubuCore.Util;

namespace Bottles.Deployment
{
    public class Recipe
    {
        private readonly string _name;
        private readonly Cache<string, HostManifest> _hosts = new Cache<string, HostManifest>(name => new HostManifest(name));
        private readonly List<string> _dependencies = new List<string>();

        public Recipe(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public HostManifest HostFor(string name)
        {
            return _hosts[name];
        }

        public IEnumerable<HostManifest> Hosts
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

        public void RegisterHost(HostManifest host)
        {
            _hosts[host.Name] = host;
        }

        public void AppendBehind(Recipe recipe)
        {
            recipe.Hosts.Each(other => _hosts[other.Name].Append(other));
        }
    }
}