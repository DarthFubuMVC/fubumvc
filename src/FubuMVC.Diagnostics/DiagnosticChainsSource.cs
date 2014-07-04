using System.Collections.Generic;
using Bottles;
using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics
{
    public class DiagnosticChainsSource : IChainSource
    {
        public IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph)
        {
            foreach (var action in findActions())
            {
                if (action.Method.Name.StartsWith("Visualize"))
                {
                    var chain = new BehaviorChain();
                    chain.AddToEnd(action);
                    chain.IsPartialOnly = true;

                    chain.Tags.Add("Diagnostics");

                    yield return chain;
                }
                else
                {
                    yield return new DiagnosticChain(action);
                }
            }
        }

        private IEnumerable<ActionCall> findActions()
        {
            var source = new ActionSource();

            PackageRegistry.PackageAssemblies.Each(a => source.Applies.ToAssembly(a));
            source.IncludeTypesNamed(name => name.EndsWith("FubuDiagnostics"));

            return source.As<IActionSource>().FindActions(null);
        }
    }
}