using System.Collections.Generic;

namespace FubuCore.Testing.DependencyAnalysis
{
    public class Bottle
    {
        IList<string> _dependencies;
        public Bottle(string name) 
        {
            Name = name;
            _dependencies = new List<string>();
        }

        public string Name { get; private set; }
        public ICollection<string> Dependencies { get { return _dependencies; } }

        public void AddDependency(string name)
        {
            _dependencies.Add(name);
        }
    }
}