using System.Linq;
using FubuMVC.Core.Content;
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
            ClassUnderTest.ConfiguredScript("jquery.js");
            ClassUnderTest.ConfiguredScript("jquery.js");
            ClassUnderTest.ConfiguredScript("jquery.js");

            SpecificationExtensions.ShouldEqual(ClassUnderTest.AllConfiguredScriptNames().Single(), "jquery.js");
        }

        [Test]
        public void do_not_double_dip_a_pagescript_file()
        {
            ClassUnderTest.PageScript("jquery.js");
            ClassUnderTest.PageScript("jquery.js");
            ClassUnderTest.PageScript("jquery.js");

            SpecificationExtensions.ShouldEqual(ClassUnderTest.AllPageScriptNames().Single(), "jquery.js");
        }

        [Test]
        public void use_file_if_exists_positive_case()
        {
            scriptExists("jquery.js");
            ClassUnderTest.UseFileIfExists("jquery.js");

            SpecificationExtensions.ShouldEqual(ClassUnderTest.AllConfiguredScriptNames().Single(), "jquery.js");
        }

        [Test]
        public void use_file_if_exists_negative_case()
        {
            scriptDoesNotExist("jquery.js");
            ClassUnderTest.UseFileIfExists("jquery.js"); 

            SpecificationExtensions.ShouldBeFalse(ClassUnderTest.AllConfiguredScriptNames().Any());
        }

    }
}