using System;
using System.Collections.Generic;
using FubuCore.Util;

namespace FubuMVC.Core.UI.Scripts
{
    public class ScriptDependencyGraph
    {
        private readonly List<Rule> _rules = new List<Rule>();
        private readonly Cache<string, List<string>> _dependencies =
            new Cache<string, List<string>>(key => new List<string>());


        public bool IsFirstDependentOnSecondScript(string first, string second)
        {
            throw new NotImplementedException();
        }


        // TODO -- Trap circulars here
        public void Compile(IScriptGraphLogger logger, ScriptGraph graph)
        {
            throw new NotImplementedException();
        }

        // Gotta watch for sets!!!
        public void AddRule(string dependent, string dependency)
        {
            _rules.Fill(new Rule(){
                Dependency = dependency,
                Dependent = dependent
            });
        }

        public IEnumerable<string> ScriptDependenciesFor(string name)
        {
            return _dependencies[name];
        }

        public class Rule
        {
            public string Dependency { get; set; }
            public string Dependent { get; set; }

            public bool Equals(Rule other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Dependency, Dependency) && Equals(other.Dependent, Dependent);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (Rule)) return false;
                return Equals((Rule) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Dependency != null ? Dependency.GetHashCode() : 0)*397) ^ (Dependent != null ? Dependent.GetHashCode() : 0);
                }
            }
        }
    }
}