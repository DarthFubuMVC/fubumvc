using System;
using System.Collections.Generic;
using System.Linq;

namespace Bottles.DependencyAnalysis
{
    public class DependencyGraph<T>
    {
        readonly DirectedGraph _cycleDetector;
        readonly IDictionary<string, T> _items;
        readonly Func<T, string> _getName;
        readonly Func<T, IEnumerable<string>> _getDependencies;


        public DependencyGraph(Func<T, string> getName, Func<T, IEnumerable<string>> getDependencies)
        {
            _cycleDetector = new DirectedGraph();
            _items = new Dictionary<string, T>();
            _getName = getName;
            _getDependencies = getDependencies;
        }

        public void RegisterItem(T item)
        {
            var name = _getName(item);
            _items.Add(name, item);
            _cycleDetector.AddNode(new Node(name));
            foreach (var dep in _getDependencies(item))
            {
                //bottle X needs bottle Y
                _cycleDetector.Connect(_getName(item), dep);
            }
        }

        public bool HasCycles()
        {
            var cycles = _cycleDetector.FindCycles().ToList();
            return cycles.Count() > 0;
        }

        public IEnumerable<string> MissingDependencies()
        {
            var registeredNames = _items.Keys.ToList();
            var neededNames = _cycleDetector.Nodes.Select(n=>n.Name).ToList();
            var missing = neededNames.Except(registeredNames);
            return missing;
        }

        public bool HasMissingDependencies()
        {
            var missing = MissingDependencies();
            return missing.Count() > 0;
        }

        public IEnumerable<T> Ordered()
        {
            return GetLoadOrder().Select(name => _items[name]);
        }

        public IEnumerable<string> GetLoadOrder()
        {
            foreach (var node in _cycleDetector.Order())
            {
                yield return node.Name;
            }

            yield break;
        }
    }
}