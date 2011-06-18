﻿using System.Linq;
using Bottles.Diagnostics;
using FubuMVC.Core.Content;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.UI.Scripts;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Scripts
{
    [TestFixture]
    public class ScriptRequirementsTester : InteractionContext<ScriptRequirements>
    {
        private void scriptExists(string name)
        {
            MockFor<IContentFolderService>().Stub(x => x.FileExists(ContentType.scripts, name))
                .Return(true);
        }

        private void scriptDoesNotExist(string name)
        {
            MockFor<IContentFolderService>().Stub(x => x.FileExists(ContentType.scripts, name))
                .Return(false);
        }

        [Test]
        public void do_not_double_dip_a_requested_file()
        {
            ClassUnderTest.Require("jquery.js");
            ClassUnderTest.Require("jquery.js");
            ClassUnderTest.Require("jquery.js");

            SpecificationExtensions.ShouldEqual(ClassUnderTest.GetScriptsToRender().Select(x => x.Name).Single(), "jquery.js");
        }

        [Test]
        public void use_file_if_exists_positive_case()
        {
            scriptExists("jquery.js");
            ClassUnderTest.UseFileIfExists("jquery.js");

            SpecificationExtensions.ShouldEqual(ClassUnderTest.GetScriptsToRender().Select(x => x.Name).Single(), "jquery.js");
        }

        [Test]
        public void use_file_if_exists_negative_case()
        {
            scriptDoesNotExist("jquery.js");
            ClassUnderTest.UseFileIfExists("jquery.js");

            SpecificationExtensions.ShouldBeFalse(ClassUnderTest.GetScriptsToRender().Any());
        }

    }

    [TestFixture]
    public class when_asking_script_requirements_for_scripts_to_write : InteractionContext<ScriptRequirements>
    {
        protected override void beforeEach()
        {
            var scriptGraph = new ScriptGraph();
            scriptGraph.Dependency("a", "b");
            scriptGraph.Dependency("a", "c");
            scriptGraph.Dependency("d", "e");
            scriptGraph.Dependency("d", "b");
            scriptGraph.CompileDependencies(new PackageLog());
            Services.Inject(scriptGraph);
        }

        [Test]
        public void should_not_write_the_same_scripts_more_than_once()
        {
            // ask for a & f, get b,c,a,f
            ClassUnderTest.Require("a"); // depends on b & c
            ClassUnderTest.Require("f"); // no dependencies

            ClassUnderTest.GetScriptsToRender().Select(x => x.Name).ShouldHaveTheSameElementsAs("b", "c", "f", "a");
            // ask for d, get d,e (not b, since it was already written)

            ClassUnderTest.Require("d"); // depends on e and b
            ClassUnderTest.GetScriptsToRender().Select(x => x.Name).ShouldHaveTheSameElementsAs("e", "d");
        }
    }
}