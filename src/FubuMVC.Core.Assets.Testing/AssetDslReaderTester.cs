using FubuMVC.Core.Assets;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class when_reading_an_ordered_set : InteractionContext<AssetDslReader>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.ReadLine("ordered set SET01 is");
            ClassUnderTest.ReadLine("D.js");
            ClassUnderTest.ReadLine("E.js");
            ClassUnderTest.ReadLine("F.js");
            ClassUnderTest.ReadLine("G.js");

        }

        [Test]
        public void should_have_added_all_the_scripts_to_the_set()
        {
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToSet("SET01", "D.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToSet("SET01", "E.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToSet("SET01", "F.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToSet("SET01", "G.js"));
        }

        [Test]
        public void uses_the_order_to_set_dependencies()
        {
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("E.js", "D.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("F.js", "E.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("G.js", "F.js"));
        }
    }


    [TestFixture]
    public class AssetDslReaderTester : InteractionContext<AssetDslReader>
    {
        [Test]
        public void preceeding()
        {
            ClassUnderTest.ReadLine("before-b preceeds b");
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Preceeding("before-b", "b"));
        }

        [Test]
        public void read_alias_happy_path()
        {
            ClassUnderTest.ReadLine("jquery is jquery.1.4.2.js");

            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Alias("jquery.1.4.2.js", "jquery"));
        }

        [Test]
        public void read_extends_happy_path()
        {
            ClassUnderTest.ReadLine("validation2.js extends validation.core.js");

            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Extension("validation2.js", "validation.core.js"));
        }

        [Test]
        public void read_requires_with_a_single_dependency_happy_path()
        {
            ClassUnderTest.ReadLine("validation.js requires jquery");
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("validation.js", "jquery"));
        }

        [Test]
        public void read_requires_with_multiple_dependencies_happy_path_1()
        {
            ClassUnderTest.ReadLine("crudForm.js requires jquery, validation.js, stateManager.js");

            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "jquery"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "validation.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "stateManager.js"));
        }


        [Test]
        public void read_requires_with_multiple_dependencies_happy_path_2()
        {
            ClassUnderTest.ReadLine("crudForm.js requires jquery,validation.js,stateManager.js");

            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "jquery"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "validation.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "stateManager.js"));
        }


        [Test]
        public void read_requires_with_multiple_dependencies_happy_path_3()
        {
            ClassUnderTest.ReadLine("crudForm.js requires jquery,validation.js, stateManager.js");

            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "jquery"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "validation.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "stateManager.js"));
        }

        [Test]
        public void read_includes_happy_path_with_multiple_entries()
        {
            ClassUnderTest.ReadLine("crud includes crudForm.js, validation.js, stateManager.js");
        
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToSet("crud", "crudForm.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToSet("crud", "validation.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToSet("crud", "stateManager.js"));
        }

        [Test]
        public void negative_case_when_multiple_names_are_passed_to_is_verb()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("crudForm.js is a,b,c,d");
            }).Message.ShouldContain("Only one name can appear on the right side of the 'is' verb");
        }

        [Test]
        public void negative_case_when_multiple_names_are_passed_to_extends_verb()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("a extends b, c, d");
            }).Message.ShouldContain("Only one name can appear on the right side of the 'extends' verb");
        }

        [Test]
        public void negative_case_when_not_enough_tokens_are_discovered()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("only one");
            }).Message.ShouldContain("Not enough tokens in the command line");
        }

        [Test]
        public void negative_case_a_wrong_verb_is_passed_in()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("a wrong b");
            }).Message.ShouldContain("'wrong' is an invalid verb");
        }

        [Test]
        public void apply_a_policy_type()
        {
            ClassUnderTest.ReadLine("apply policy CombineAllScriptFiles");

            MockFor<IAssetRegistration>().AssertWasCalled(x => x.ApplyPolicy("CombineAllScriptFiles"));
        }

        [Test]
        public void apply_policy_type_integrated_with_asset_graph()
        {
            var graph = new AssetGraph();
            var reader = new AssetDslReader(graph);

            reader.ReadLine("apply policy " + typeof(AssetGraphTester.FakeComboPolicy).AssemblyQualifiedName);

            graph.PolicyTypes.ShouldHaveTheSameElementsAs(typeof(AssetGraphTester.FakeComboPolicy));
        }

        [Test]
        public void apply_policy_type_negative_1()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("apply something something");
            });
        }

        [Test]
        public void combine_files_1()
        {
            ClassUnderTest.ReadLine("combine a.js, b.js, c.js as abc.js");

            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToCombination("abc.js", "a.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToCombination("abc.js", "b.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToCombination("abc.js", "c.js"));
        }

        [Test]
        public void combine_files_2()
        {
            ClassUnderTest.ReadLine("combine a.js, b.js,c.js as abc.js");

            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToCombination("abc.js", "a.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToCombination("abc.js", "b.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToCombination("abc.js", "c.js"));
        }

        [Test]
        public void combine_files_3()
        {
            ClassUnderTest.ReadLine("combine a.js,b.js,c.js as abc.js");

            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToCombination("abc.js", "a.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToCombination("abc.js", "b.js"));
            MockFor<IAssetRegistration>().AssertWasCalled(x => x.AddToCombination("abc.js", "c.js"));
        }

        [Test]
        public void combine_without_as_throws()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("combine a.js,b.js,c.js");
            });
        }

        [Test]
        public void combine_without_combo_name_throws()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("combine a.js,b.js,c.js as");
            });
        }
    }
}