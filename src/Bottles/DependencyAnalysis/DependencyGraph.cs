using System.Collections.Generic;
using System.Linq;

namespace Bottles.DependencyAnalysis
{
    public class DependencyGraph
    {
        readonly DirectedGraph _cycleDetector;
        readonly IDictionary<Urn, Bottle> _bottles;

        public DependencyGraph()
        {
            _cycleDetector = new DirectedGraph();
            _bottles = new Dictionary<Urn, Bottle>();
        }

        public void AddBottle(Bottle bottle)
        {
            _bottles.Add(bottle.Name, bottle);
            _cycleDetector.AddNode(new Node(bottle.Name));
            foreach (var dep in bottle.Dependencies)
            {
                //bottle X needs bottle Y
                _cycleDetector.Connect(bottle.Name, dep);
            }
        }

        public bool HasCycles()
        {
            var cycles = _cycleDetector.FindCycles().ToList();
            return cycles.Count() > 0;
        }
        public IEnumerable<Urn> MissingDependencies()
        {
            var registeredNames = _bottles.Keys.ToList();
            var neededNames = _cycleDetector.Nodes.Select(n=>n.Name).ToList();
            var missing = neededNames.Except(registeredNames);
            return missing;
        }

        public bool HasMissingDependencies()
        {
            var missing = MissingDependencies();
            return missing.Count() > 0;
        }

        public IEnumerable<Urn> GetLoadOrder()
        {
            foreach (var node in _cycleDetector.Order())
            {
                yield return node.Name;
            }

            yield break;
        }
    }
}