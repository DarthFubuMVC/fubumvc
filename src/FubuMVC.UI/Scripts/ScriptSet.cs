using System.Collections;
using System.Collections.Generic;

namespace FubuMVC.UI.Scripts
{
    public class ScriptSet : IEnumerable<Script>
    {
        private readonly string _name;
        private readonly List<Script> _scripts;

        public ScriptSet(string name)
        {
            _name = name;
            _scripts = new List<Script>();
        }

        public string Name
        {
            get { return _name; }
        }

        public void AddScript(Script script)
        {
            _scripts.Fill(script);
        }

        public bool Contains(Script script)
        {
            return _scripts.Contains(script);
        }

        public IEnumerator<Script> GetEnumerator()
        {
            return _scripts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ScriptSet)) return false;
            return Equals((ScriptSet) obj);
        }

        public bool Equals(ScriptSet other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name) && _scripts.IsEqualTo(other._scripts);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_name != null ? _name.GetHashCode() : 0)*397) ^ (_scripts != null ? _scripts.GetHashCode() : 0);
            }
        }
    }
}