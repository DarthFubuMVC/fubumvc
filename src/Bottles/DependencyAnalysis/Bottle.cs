using System.Collections.Generic;

namespace Bottles.DependencyAnalysis
{
    public class Bottle
    {
        IList<Urn> _dependencies;
        public Bottle(string name) : this(new Urn(name))
        {
            
        }
        public Bottle(Urn name)
        {
            Name = name;
            _dependencies = new List<Urn>();
        }

        public Urn Name { get; private set; }
        public ICollection<Urn> Dependencies { get { return _dependencies; } }

        public void AddDependency(string name)
        {
            AddDependency(new Urn(name));
        }
        public void AddDependency(Urn name)
        {
            _dependencies.Add(name);
        }
    }
}