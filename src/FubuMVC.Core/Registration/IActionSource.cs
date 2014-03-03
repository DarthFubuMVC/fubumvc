using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    /// <summary>
    /// Implementations of this contract inspect a given <see cref="TypePool"/>
    /// and provide any number of <see cref="ActionCall"/> instances.
    /// </summary>
    public interface IActionSource
    {
        IEnumerable<ActionCall> FindActions(Assembly applicationAssembly);
    }

    public interface IChainSource
    {
        IEnumerable<BehaviorChain> BuildChains(SettingsCollection settings);
    }

    public class ActionSourceAggregator : IChainSource
    {
        private readonly Assembly _applicationAssembly;
        private readonly IList<IActionSource> _sources = new List<IActionSource>(); 

        public ActionSourceAggregator(Assembly applicationAssembly)
        {
            _applicationAssembly = applicationAssembly;
        }

        public IEnumerable<BehaviorChain> BuildChains(SettingsCollection settings)
        {
            var sources = _sources.Any() ? _sources : new IActionSource[] {new EndpointActionSource()};

            var actions = sources.SelectMany(x => x.FindActions(_applicationAssembly))
                .Distinct();

            foreach (var action in actions)
            {
                var chain = new BehaviorChain();
                chain.AddToEnd(action);

                yield return chain;
            }
        }

        public void Add(IActionSource source)
        {
            _sources.Add(source);
        }

        public IEnumerable<IActionSource> Sources
        {
            get { return _sources; }
        }

        public Assembly ApplicationAssembly
        {
            get { return _applicationAssembly; }
        }
    }
}