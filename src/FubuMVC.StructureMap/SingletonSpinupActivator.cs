using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using StructureMap;

namespace FubuMVC.StructureMap
{
    public class SingletonSpinupActivator : IActivator
    {
        private readonly IContainer _container;

        public SingletonSpinupActivator(IContainer container)
        {
            _container = container;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            // Remove this method when the issue is closed 
            // http://github.com/structuremap/structuremap/issues#issue/3
            var allSingletons =
                _container.Model.PluginTypes.Where(x => x.Lifecycle == InstanceScope.Singleton.ToString());

            foreach (var pluginType in allSingletons)
            {
                _container.GetInstance(pluginType.PluginType);
            }
        }
    }
}