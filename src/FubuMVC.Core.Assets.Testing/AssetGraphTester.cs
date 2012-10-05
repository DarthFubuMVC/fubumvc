using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetGraphTester
    {
        private AssetGraph theGraph;
        private bool _compiled;

        [SetUp] 
        public void SetUp()
        {
            _compiled = false;
            theGraph = new AssetGraph(); 
        }

        [Test]
        public void asset_files_key_equals()
        {
            var key1 = new AssetGraph.AssetFilesKey(new string[]{"a", "b", "c"});
            var key2 = new AssetGraph.AssetFilesKey(new string[]{"a", "b", "c"});
            var key3 = new AssetGraph.AssetFilesKey(new string[]{"a", "c", "b"});
            var key4 = new AssetGraph.AssetFilesKey(new string[]{"a", "c", "d"});

            key1.ShouldEqual(key2);
            key2.ShouldEqual(key3);
            key3.ShouldEqual(key1);

            key1.ShouldNotEqual(key4);

            key1.GetHashCode().ShouldEqual(key2.GetHashCode());
            key1.GetHashCode().ShouldEqual(key3.GetHashCode());
            key1.GetHashCode().ShouldNotEqual(key4.GetHashCode());
        }

        [Test]
        public void add_to_combination()
        {
            theGraph.AddToCombination("c1", "a.js");
            theGraph.NamesForCombination("c1").ShouldHaveTheSameElementsAs("a.js");

            theGraph.AddToCombination("c2", "b.js, c.js, d.js");
            theGraph.NamesForCombination("c2").ShouldHaveTheSameElementsAs("b.js", "c.js", "d.js");

            theGraph.AddToCombination("c3", "b.js,c.js,d.js");
            theGraph.NamesForCombination("c3").ShouldHaveTheSameElementsAs("b.js", "c.js", "d.js");
        }

        [Test]
        public void add_to_combination_should_unalias_names()
        {
            theGraph.Alias("a.js", "a");
            theGraph.Alias("b.js", "b");

            theGraph.AddToCombination("c1", "a,b,c.js");

            theGraph.NamesForCombination("c1").ShouldHaveTheSameElementsAs("a.js", "b.js", "c.js");
        }

        [Test]
        public void add_to_combination_should_throw_when_combination_does_not_already_exist_and_names_for_more_than_one_mime_type()
        {
            theGraph.Alias("b.css", "b");

            Exception<InvalidOperationException>.ShouldBeThrownBy(() => theGraph.AddToCombination("c1", "a.js,b"));
        }

        [Test]
        public void add_to_combination_should_throw_when_combination_already_exists_and_names_for_do_not_all_match_mime_type()
        {
            theGraph.AddToCombination("c1", "a.js");
            
            theGraph.Alias("b.css", "b");

            Exception<InvalidOperationException>.ShouldBeThrownBy(() => theGraph.AddToCombination("c1", "b,c.js"));
        }

        [Test]
        public void apply_full_assembly_qualified_type_name()
        {
            theGraph.ApplyPolicy(typeof(FakeComboPolicy).AssemblyQualifiedName);

            theGraph.PolicyTypes.ShouldHaveTheSameElementsAs(typeof(FakeComboPolicy));
        }

        [Test]
        public void apply_can_find_built_in_policy_types_by_name_only()
        {
            theGraph.ApplyPolicy(typeof(CombineAllScriptFiles).Name);
            theGraph.ApplyPolicy(typeof(CombineAllStylesheets).Name);

            theGraph.PolicyTypes.ShouldHaveTheSameElementsAs(typeof(CombineAllScriptFiles), typeof(CombineAllStylesheets));
        }

        [Test]
        public void apply_is_a_fill_to_prevent_duplicates()
        {
            theGraph.ApplyPolicy(typeof(FakeComboPolicy).AssemblyQualifiedName);
            theGraph.ApplyPolicy(typeof(FakeComboPolicy).AssemblyQualifiedName);
            theGraph.ApplyPolicy(typeof(FakeComboPolicy).AssemblyQualifiedName);

            theGraph.PolicyTypes.ShouldHaveTheSameElementsAs(typeof(FakeComboPolicy));
        }

        [Test]
        public void apply_will_throw_argument_out_of_range_if_the_type_cannot_be_found()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                theGraph.ApplyPolicy("not a type");
            });
        }

        public class FakeComboPolicy : ICombinationPolicy
        {
            public MimeType MimeType
            {
                get { throw new NotImplementedException(); }
            }

            public IEnumerable<AssetFileCombination> DetermineCombinations(AssetTagPlan plan)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void get_a_set_for_an_empty_graph_returns_an_empty_set_of_that_name()
        {
            theGraph.AssetSetFor("some name")
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

            IEnumerable<string> scripts = theGraph.GetAssets(names).Select(x => x.Name);
            scripts.Each(x => Debug.WriteLine(x));
            return scripts;
        }


        [Test]
        public void find_object_by_name_and_the_object_does_not_exist_use_script_by_name()
        {
            theGraph.ObjectFor("Name.js").ShouldBeOfType<IFileDependency>()
                .Name.ShouldEqual("Name.js");
        }

        [Test]
        public void find_object_by_alias()
        {
            theGraph.Alias("Name.js", "Name");
            theGraph.ObjectFor("Name").ShouldBeOfType<IFileDependency>()
                .Name.ShouldEqual("Name.js");
        }   

        [Test]
        public void only_the_very_first_alias_is_registered()
        {
            theGraph.Alias("jquery5.1.11.1.js", "jquery");
            theGraph.Alias("jquery.1.8.1.js", "jquery");
            theGraph.Alias("jquery.1.9.1.js", "jquery");

            theGraph.ObjectFor("jquery").ShouldBeOfType<IFileDependency>()
                .Name.ShouldEqual("jquery5.1.11.1.js");
        }


        [Test]
        public void only_the_very_first_alias_is_registered_2()
        {
            theGraph.Alias("jquery.1.1.1.js", "jquery");
            theGraph.Alias("jquery.1.8.1.js", "jquery");
            theGraph.Alias("jquery.1.9.1.js", "jquery");

            theGraph.ObjectFor("jquery").ShouldBeOfType<IFileDependency>()
                .Name.ShouldEqual("jquery.1.1.1.js");
        }

        [Test]
        public void find_a_set_by_name()
        {
            theGraph.AddToSet("SetA", "A");
            theGraph.ObjectFor("SetA").ShouldBeOfType<AssetSet>();
        }

        [Test]
        public void find_a_set_by_alias()
        {
            theGraph.AddToSet("SetA", "A");
            theGraph.Alias("SetA", "SetA-Alias");
            theGraph.ObjectFor("SetA-Alias").ShouldBeOfType<AssetSet>().Name.ShouldEqual("SetA");
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

            var a = theGraph.FileDependencyFor("A");
            var b = theGraph.FileDependencyFor("B");
            var c = theGraph.FileDependencyFor("C");
            var d = theGraph.FileDependencyFor("D");
            var f = theGraph.FileDependencyFor("F");

            f.MustBeAfter(c).ShouldBeFalse();
            b.MustBeAfter(c).ShouldBeFalse();

            a.MustBeAfter(b).ShouldBeTrue();
            a.MustBeAfter(c).ShouldBeTrue();
            d.MustBeAfter(a).ShouldBeTrue();
            d.MustBeAfter(b).ShouldBeTrue();
            d.MustBeAfter(c).ShouldBeTrue();
            f.MustBeAfter(b).ShouldBeTrue();


            IEnumerable<string> theNames = theGraph.GetAssets(new string[]{"A"}).Select(x => x.Name).ToList();
            theNames.Each(x => Debug.WriteLine(x));
            theNames.ShouldHaveTheSameElementsAs("B", "C", "F", "A", "D");
        }

        [Test]
        public void preceeding()
        {
            theGraph.Preceeding("before-b", "b");
            theGraph.CompileDependencies(new PackageLog());

            theGraph.FileDependencyFor("before-b").MustBeAfter(theGraph.FileDependencyFor("b")).ShouldBeFalse();
            theGraph.FileDependencyFor("b").MustBeAfter(theGraph.FileDependencyFor("before-b")).ShouldBeTrue();

            ScriptNamesFor("b").ShouldHaveTheSameElementsAs("b");
            ScriptNamesFor("b", "before-b").ShouldHaveTheSameElementsAs("before-b", "b");
            ScriptNamesFor("before-b").ShouldHaveTheSameElementsAs("before-b");
        }

        [Test]
        public void sets_names()
        {
            theGraph.AddToSet("setA", "a-1.js");
            theGraph.AddToSet("setA", "a-2.js");
            theGraph.AddToSet("setB", "b-1.js");
            theGraph.AddToSet("setB", "b-2.js");

            var sets = new List<string>();
            theGraph.ForEachSetName(sets.Add);

            sets.ShouldEqual(new[] { "setA", "setB" });
        }
    }

    [TestFixture]
    public class sorting_scripts_tester
    {
        [SetUp]
        public void SetUp()
        {
            graph = new AssetGraph();
            var reader = new AssetDslReader(graph);
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
            var d = graph.FileDependencyFor("D");
            d.MustBeAfter(graph.FileDependencyFor("D1")).ShouldBeTrue();
            d.MustBeAfter(graph.FileDependencyFor("D2")).ShouldBeTrue();
        
            graph.FileDependencyFor("D1").MustBeAfter(d).ShouldBeFalse();
            graph.FileDependencyFor("D2").MustBeAfter(d).ShouldBeFalse();
        }


        [Test]
        public void fetch_for_a_set_that_includes_another_set()
        {
            var a = graph.FileDependencyFor("A");
            var b = graph.FileDependencyFor("B");
            var c = graph.FileDependencyFor("C");
            var c1 = graph.FileDependencyFor("C-1");
            var crudForm = graph.FileDependencyFor("crudForm.js");
            var validation = graph.FileDependencyFor("validation.js");

            a.MustBeAfter(crudForm).ShouldBeTrue();
            a.MustBeAfter(validation).ShouldBeTrue();
            a.MustBeAfter(b).ShouldBeFalse();
            b.MustBeAfter(a).ShouldBeFalse();

            b.MustBeAfter(crudForm).ShouldBeFalse();
            crudForm.MustBeAfter(b).ShouldBeFalse();

            IEnumerable<string> theNames = graph.GetAssets(new string[] { "1" }).Select(x => x.Name).ToList();
            theNames.Each(x => Debug.WriteLine(x));
        }


        private AssetGraph graph;
    }
}