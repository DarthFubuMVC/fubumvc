using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    public class SharingGraph : ISharingRegistration
    {
        private readonly List<GlobalDependency> _globals = new List<GlobalDependency>();
        private readonly List<DependencyRule> _dependencyRules = new List<DependencyRule>(); 

        public void Global(string global)
        {
            _globals.Fill(new GlobalDependency
            {
                Dependency = global                                          
            });
        }

        public void Dependency(string dependent, string dependency)
        {
            if(dependent == dependency) return;

            _dependencyRules.Fill(new DependencyRule
            {
                Dependent = dependent,
                Dependency = dependency                                          
            });
        }

        public void CompileDependencies(params string[] provenances)
        {
            _globals.Each(p => provenances.Each(pr => Dependency(pr, p.Dependency)));
        }

        public IEnumerable<string> SharingsFor(string provenance)
        {
            return _dependencyRules
                .Where(r => r.Dependent == provenance)
                .Select(r => r.Dependency);
        } 

        public class GlobalDependency
        {            
            public string Dependency { get; set; }
        }

        public class DependencyRule
        {
            public string Dependency { get; set; }
            public string Dependent { get; set; }

            public bool Equals(DependencyRule other)
            {
                return !ReferenceEquals(null, other) && (ReferenceEquals(this, other) ||
                    Equals(other.Dependency, Dependency) && Equals(other.Dependent, Dependent));
            }

            public override bool Equals(object obj)
            {
                return !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || 
                    obj.GetType() == typeof (DependencyRule) && Equals((DependencyRule) obj));
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Dependency != null ? Dependency.GetHashCode() : 0)*397) ^ 
                        (Dependent != null ? Dependent.GetHashCode() : 0);
                }
            }
        }
    }
}