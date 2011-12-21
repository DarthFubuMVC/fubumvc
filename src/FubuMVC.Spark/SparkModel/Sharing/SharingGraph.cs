using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    public interface ISharingGraph
    {
        IEnumerable<string> SharingsFor(string provenance);
    }

    public class SharingGraph : ISharingRegistration, ISharingGraph
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

            public bool Equals(GlobalDependency other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Dependency, Dependency);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (GlobalDependency)) return false;
                return Equals((GlobalDependency) obj);
            }

            public override int GetHashCode()
            {
                return (Dependency != null ? Dependency.GetHashCode() : 0);
            }
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