using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Packaging;
using FubuTestingSupport;
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
                theGraph.CompileDependencies(new PackageLog());
                _compiled = true;
            }

            IEnumerable<string> scripts = theGraph.GetScripts(names).Select(x => x.Name);
            scripts.Each(x => Debug.WriteLine(x));
            return scripts;
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
            theGraph.CompileDependencies(new PackageLog());

            ScriptNamesFor("a").ShouldHaveTheSameElementsAs("b", "c", "a");
        }

        [Test]
        public void find_scripts_with_an_extension()
        {
            theGraph.Extension("a1", "a");
            ScriptNamesFor("a", "c", "b").ShouldHaveTheSameElementsAs("a", "b", "c", "a1");

        }

        [Test]
        public void use_a_dependency_on_a_set()
        {
            theGraph.AddToSet("basic", "jquery");
            theGraph.AddToSet("basic", "windowManager");
            theGraph.Dependency("script1", "basic");

            
            ScriptNamesFor("script1").ShouldHaveTheSameElementsAs("jquery", "windowManager", "script1");
        }


        [Test]
        public void ordering_test()
        {
            theGraph.Dependency("A", "B");
            theGraph.Dependency("A", "C");
            theGraph.Extension("D", "A");
            theGraph.Extension("F", "B");

            theGraph.CompileDependencies(new PackageLog());

            var a = theGraph.ScriptFor("A");
            var b = theGraph.ScriptFor("B");
            var c = theGraph.ScriptFor("C");
            var d = theGraph.ScriptFor("D");
            var f = theGraph.ScriptFor("F");

            f.MustBeAfter(c).ShouldBeFalse();
            b.MustBeAfter(c).ShouldBeFalse();

            a.MustBeAfter(b).ShouldBeTrue();
            a.MustBeAfter(c).ShouldBeTrue();
            d.MustBeAfter(a).ShouldBeTrue();
            d.MustBeAfter(b).ShouldBeTrue();
            d.MustBeAfter(c).ShouldBeTrue();
            f.MustBeAfter(b).ShouldBeTrue();


            IEnumerable<string> theNames = theGraph.GetScripts(new string[]{"A"}).Select(x => x.Name).ToList();
            theNames.Each(x => Debug.WriteLine(x));
            theNames.ShouldHaveTheSameElementsAs("B", "C", "F", "A", "D");
        }

        [Test]
        public void preceeding()
        {
            theGraph.Preceeding("before-b", "b");
            theGraph.CompileDependencies(new PackageLog());

            theGraph.ScriptFor("before-b").MustBeAfter(theGraph.ScriptFor("b")).ShouldBeFalse();
            theGraph.ScriptFor("b").MustBeAfter(theGraph.ScriptFor("before-b")).ShouldBeTrue();

            ScriptNamesFor("b").ShouldHaveTheSameElementsAs("b");
            ScriptNamesFor("b", "before-b").ShouldHaveTheSameElementsAs("before-b", "b");
            ScriptNamesFor("before-b").ShouldHaveTheSameElementsAs("before-b");
        }
    }

    [TestFixture]
    public class sorting_scripts_tester
    {
        [SetUp]
        public void SetUp()
        {
            graph = new ScriptGraph();
            var reader = new ScriptDslReader(graph);
            reader.ReadLine("1 includes A,B,C");
            reader.ReadLine("2 includes C,D");
            reader.ReadLine("3 includes 1,E");
            reader.ReadLine("D requires D1,D2");
            reader.ReadLine("3 requires 4");
            reader.ReadLine("4 includes jquery,validation.js");
            reader.ReadLine("Combo includes 1,2");
            reader.ReadLine("C-1 extends C");
            reader.ReadLine("crud includes crudForm.js,validation.js");
            reader.ReadLine("A requires crud");
            graph.CompileDependencies(new PackageLog());
        }

        [Test]
        public void d_should_be_after_d1_and_d2()
        {
            var d = graph.ScriptFor("D");
            d.MustBeAfter(graph.ScriptFor("D1")).ShouldBeTrue();
            d.MustBeAfter(graph.ScriptFor("D2")).ShouldBeTrue();
        
            graph.ScriptFor("D1").MustBeAfter(d).ShouldBeFalse();
            graph.ScriptFor("D2").MustBeAfter(d).ShouldBeFalse();
        }


        [Test]
        public void fetch_for_a_set_that_includes_another_set()
        {
            var a = graph.ScriptFor("A");
            var b = graph.ScriptFor("B");
            var c = graph.ScriptFor("C");
            var c1 = graph.ScriptFor("C-1");
            var crudForm = graph.ScriptFor("crudForm.js");
            var validation = graph.ScriptFor("validation.js");

            a.MustBeAfter(crudForm).ShouldBeTrue();
            a.MustBeAfter(validation).ShouldBeTrue();
            a.MustBeAfter(b).ShouldBeFalse();
            b.MustBeAfter(a).ShouldBeFalse();

            b.MustBeAfter(crudForm).ShouldBeFalse();
            crudForm.MustBeAfter(b).ShouldBeFalse();

            IEnumerable<string> theNames = graph.GetScripts(new string[] { "1" }).Select(x => x.Name).ToList();
            theNames.Each(x => Debug.WriteLine(x));
        }


        private ScriptGraph graph;
    }
}