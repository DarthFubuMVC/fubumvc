using System.Collections.Generic;

namespace FubuMVC.UI.Scripts
{
    public interface IScriptManager
    {
        
    }

    public class Test
    {
        public Test()
        {
            Scan(x =>
                     {
                         x.IncludeDirectory("content/scripts");
                         x.IncludeDirectory("packages");
                         x.IncludeSubdirectories();
                     });

            Script("content/scripts/lib/jquery/jquery.validate.js")
                .DependsOn("jquery.min.js");

            Override("scripts/pages/create-case.js")
                .With("packages/x/custom-create-case.js");

            Script("packages/x/custom-create-case.js")
                .DependsOn("custom-create-case-dependency.js");
        }
    }

    public class Script
    {
        private readonly string _key;
        private readonly IList<Script> _dependencies;

        public Script(string key)
        {
            _key = key;
            _dependencies = new List<Script>();
        }

        public string Key
        {
            get { return _key; }
        }

        public IEnumerable<Script> Dependencies { get { return _dependencies; } }

        public void AddDependency(Script script)
        {
            _dependencies.Add(script);
        }
    }
}