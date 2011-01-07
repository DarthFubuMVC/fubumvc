using System.Collections.Generic;
using FubuMVC.Core.UI.Scripts;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Scripts
{
    [TestFixture]
    public class ScriptDslReaderTester : InteractionContext<ScriptDslReader>
    {
        [Test]
        public void read_alias_happy_path()
        {
            ClassUnderTest.ReadLine("jquery is jquery.1.4.2.js");

            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Alias("jquery.1.4.2.js", "jquery"));
        }

        [Test]
        public void read_extends_happy_path()
        {
            ClassUnderTest.ReadLine("validation2.js extends validation.core.js");

            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Extension("validation2.js", "validation.core.js"));
        }

        [Test]
        public void read_requires_with_a_single_dependency_happy_path()
        {
            ClassUnderTest.ReadLine("validation.js requires jquery");
            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Dependency("validation.js", "jquery"));
        }

        [Test]
        public void read_requires_with_multiple_dependencies_happy_path_1()
        {
            ClassUnderTest.ReadLine("crudForm.js requires jquery, validation.js, stateManager.js");

            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "jquery"));
            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "validation.js"));
            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "stateManager.js"));
        }


        [Test]
        public void read_requires_with_multiple_dependencies_happy_path_2()
        {
            ClassUnderTest.ReadLine("crudForm.js requires jquery,validation.js,stateManager.js");

            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "jquery"));
            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "validation.js"));
            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "stateManager.js"));
        }


        [Test]
        public void read_requires_with_multiple_dependencies_happy_path_3()
        {
            ClassUnderTest.ReadLine("crudForm.js requires jquery,validation.js, stateManager.js");

            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "jquery"));
            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "validation.js"));
            MockFor<IScriptRegistration>().AssertWasCalled(x => x.Dependency("crudForm.js", "stateManager.js"));
        }

        [Test]
        public void read_includes_happy_path_with_multiple_entries()
        {
            ClassUnderTest.ReadLine("crud includes crudForm.js, validation.js, stateManager.js");
        
            MockFor<IScriptRegistration>().AssertWasCalled(x => x.AddToSet("crud", "crudForm.js"));
            MockFor<IScriptRegistration>().AssertWasCalled(x => x.AddToSet("crud", "validation.js"));
            MockFor<IScriptRegistration>().AssertWasCalled(x => x.AddToSet("crud", "stateManager.js"));
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
    }
}