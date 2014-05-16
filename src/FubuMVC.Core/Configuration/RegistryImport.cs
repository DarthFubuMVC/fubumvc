using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Configuration
{
    public class RegistryImport : IChainSource
    {
        private BehaviorGraph _behaviorGraph;
        public string Prefix { get; set; }
        public FubuRegistry Registry { get; set; }

        public void InitializeSettings(BehaviorGraph parentGraph)
        {
            _behaviorGraph = new BehaviorGraph(parentGraph.Settings)
            {
                ApplicationAssembly = Registry.ApplicationAssembly
            };

            Registry.Config.Imports.Each(x => x.InitializeSettings(parentGraph));
            
            Registry.Config.Settings.Each(x => x.Alter(_behaviorGraph.Settings));
        }

        public IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph)
        {
            return PackageRegistry.Timer.Record("Building Imported Chains for " + Registry, () => {
                Registry.Config.BuildLocal(_behaviorGraph);
                if (Prefix.IsNotEmpty())
                {
                    _behaviorGraph.Behaviors.OfType<RoutedChain>().Each(x => x.Route.Prepend(Prefix));
                }

                return _behaviorGraph.Behaviors;
            });
        }

        public bool Equals(RegistryImport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Prefix, Prefix) && Equals(other.Registry.GetType(), Registry.GetType());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RegistryImport)) return false;
            return Equals((RegistryImport) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Prefix != null ? Prefix.GetHashCode() : 0)*397) ^
                       (Registry.GetType() != null ? Registry.GetType().GetHashCode() : 0);
            }
        }


        public override string ToString()
        {
            return string.Format("Registry: {0}", Registry);
        }
    }
}