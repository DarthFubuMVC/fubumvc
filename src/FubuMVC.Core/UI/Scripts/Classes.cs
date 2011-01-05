using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using HtmlTags;

namespace FubuMVC.Core.UI.Scripts
{
    /*
     *  TODO --
     *  Diagnostics?  Are all dependencies, aliases, extensions, sets valid?
     *  What if?
     *  Everything lazy
     * 
     * 
     * 
     * Aliases can come in any order!!  ?
     */


    public interface IScriptGraphLogger
    {
    }




    // ScriptGraph is partially tested by StoryTeller
    public class ScriptGraph : IComparer<IScript>
    {
        private readonly IScriptFinder _finder;

        private readonly Cache<string, IScriptObject> _objects = new Cache<string, IScriptObject>();
        private readonly Cache<string, ScriptSet> _sets = new Cache<string, ScriptSet>();
        private readonly List<ScriptExtension> _extenders = new List<ScriptExtension>();
        private readonly List<ScriptRule> _rules = new List<ScriptRule>();

        public ScriptGraph(IScriptFinder finder)
        {
            _finder = finder;

            _sets.OnMissing = name => new ScriptSet{
                Name = name
            };

            _sets.OnAddition = (@set) =>
            {
                _objects[@set.Name] = @set;
            };


            _objects.OnMissing = name =>
            {
                return 
                    _objects.GetAll().FirstOrDefault(x => x.Matches(name)) 
                    ?? 
                    new ScriptProxy(name, _finder);
            };

        }

        /*
         * 1.) if dependency
         * 2.) if extension
         * 3.) Just name
         */

        public int Compare(IScript x, IScript y)
        {
            throw new NotImplementedException();
        }

        public void Alias(string name, string alias)
        {
            _objects[name].AddAlias(alias);
        }

        public void Dependency(string dependent, string dependency)
        {
            _rules.Fill(new ScriptRule()
            {
                Dependency = dependency,
                Dependent = dependent
            });
        }

        public void Extension(string extender, string @base)
        {
            _extenders.Add(new ScriptExtension(){
                Base = @base,
                Extender = extender
            });
        }

        public void AddToSet(string setName, string name)
        {
            _sets[setName].Add(name);
        }

        public IEnumerable<IScript> GetScripts(IEnumerable<string> names)
        {
            // ORDER LATER!!!

            return names
                .Select(name => _objects[name])
                .SelectMany(x => x.AllScripts())
                .Distinct();

            //throw new NotImplementedException();
        }

        public ScriptSet ScriptSetFor(string someName)
        {
            return _sets[someName];
        }


        // TODO -- try to find circular dependencies
        public void CompileDependencies(IScriptGraphLogger logger)
        {
            /*
             * 1.) "Canonicize" rules
             * 
             * 
             * 
             * 
             */
        }

        // Find by name or by alias
        public IScriptObject ObjectFor(string name)
        {
            return _objects[name];
        }

        public IEnumerable<string> ScriptDependenciesFor(string name)
        {
            throw new NotImplementedException();
        }
    }

    public class ScriptExtension
    {
        public string Base { get; set; }
        public string Extender { get; set; }
    }

    public class ScriptRule : IComparable<ScriptRule>
    {
        public string Dependency { get; set; }
        public string Dependent { get; set; }



        public bool Equals(ScriptRule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Dependency, Dependency) && Equals(other.Dependent, Dependent);
        }

        public int CompareTo(ScriptRule other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ScriptRule)) return false;
            return Equals((ScriptRule)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Dependency != null ? Dependency.GetHashCode() : 0) * 397) ^ (Dependent != null ? Dependent.GetHashCode() : 0);
            }
        }
    }


    public class Script : ScriptObjectBase, IScript
    {
        public string FileName { get; set; }
        public string Url { get; set; }

        public override IEnumerable<IScript> AllScripts()
        {
            // TODO -- dependencies and extensions
            yield return this;
        }

        public string ReadAll()
        {
            throw new NotImplementedException();
        }

        public HtmlTag CreateScriptTag()
        {
            throw new NotImplementedException();
        }

        public bool ShouldBeAfter(IScript script)
        {
            throw new NotImplementedException();
        }
    }

    public interface IScriptFinder
    {
        IScript Find(string name);
    }
}