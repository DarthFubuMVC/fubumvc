using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.UI.Scripts;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Scripts
{
    [TestFixture]
    public class ScriptGraphTester
    {
        private ScriptGraph theGraph;
        private bool _compiled;

        [SetUp] 
        public void SetUp()
        {
            _compiled = false;
            theGraph = new ScriptGraph(); 
        }

        [Test]
        public void get_a_set_for_an_empty_graph_returns_an_empty_set_of_that_name()
        {
            theGraph.ScriptSetFor("some name")
                .ShouldNotBeNull()
                .Name.ShouldEqual("some name");
        }

        private IEnumerable<string> ScriptNamesFor(params string[] names)
        {
            if (!_compiled)
            {
                theGraph.CompileDependencies(new PackageRegistryLog());
                _compiled = true;
            }

            return theGraph.GetScripts(names).Select(x => x.Name);
        }


        [Test]
        public void find_object_by_name_and_the_object_does_not_exist_use_script_by_name()
        {
            theGraph.ObjectFor("Name.js").ShouldBeOfType<IScript>()
                .Name.ShouldEqual("Name.js");
        }

        [Test]
        public void find_object_by_alias()
        {
            theGraph.Alias("Name.js", "Name");
            theGraph.ObjectFor("Name").ShouldBeOfType<IScript>()
                .Name.ShouldEqual("Name.js");
        }

        [Test]
        public void find_a_set_by_name()
        {
            theGraph.AddToSet("SetA", "A");
            theGraph.ObjectFor("SetA").ShouldBeOfType<ScriptSet>();
        }

        [Test]
        public void find_a_set_by_alias()
        {
            theGraph.AddToSet("SetA", "A");
            theGraph.Alias("SetA", "SetA-Alias");
            theGraph.ObjectFor("SetA-Alias").ShouldBeOfType<ScriptSet>().Name.ShouldEqual("SetA");
        }

        [Test]
        public void find_scripts_all_files()
        {
            ScriptNamesFor("a", "b", "c").ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void find_scripts_all_files_orders_by_filename_if_no_other_dependency()
        {
            ScriptNamesFor("a", "c", "b").ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void find_scripts_all_files_uses_dependencies_for_ordering()
        {
            theGraph.Dependency("a", "b");
            theGraph.Dependency("a", "c");

            ScriptNamesFor("a").ShouldHaveTheSameElementsAs("b", "c", "a");
        }

        [Test]
        public void find_scripts_with_an_extension()
        {
            theGraph.Extension("a1", "a");
            ScriptNamesFor("a", "c", "b").ShouldHaveTheSameElementsAs("a", "a1", "b", "c");

        }

    }
}