using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
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

        public Task<BehaviorChain[]> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            return Task.Factory.StartNew(() =>
            {
                return timer.Record("Building Imported Chains for " + Registry, () =>
                {
                    Registry.Config.BuildLocal(_behaviorGraph, timer);
                    if (Prefix.IsNotEmpty())
                    {
                        _behaviorGraph.Chains.OfType<RoutedChain>().Each(x => x.Route.Prepend(Prefix));
                    }

                    return _behaviorGraph.Chains.ToArray();
                });
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