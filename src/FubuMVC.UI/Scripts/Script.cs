using System.Collections.Generic;

namespace FubuMVC.UI.Scripts
{
    public class Script
    {
        private readonly string _name;
        private string _path;
        private readonly IList<Script> _dependencies;
        private readonly IList<Script> _extensions;

        public Script(string name, string path)
        {
            _name = name;
            _path = path;
            _dependencies = new List<Script>();
            _extensions = new List<Script>();
        }

        public string Name
        {
            get { return _name; }
        }

        public string Path
        {
            get { return _path; }
        }

        public void ResetPath(string newPath)
        {
            _path = newPath;
        }

        public IEnumerable<Script> Dependencies { get { return _dependencies; } }

        public IEnumerable<Script> Extensions { get { return _extensions; } }

        public void AddDependency(Script script)
        {
            _dependencies.Add(script);
        }

        public void AddExtension(Script script)
        {
            _extensions.Add(script);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Script)) return false;
            return Equals((Script) obj);
        }

        public bool Equals(Script other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }
    }
}