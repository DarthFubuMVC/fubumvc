using System;
using System.Linq;
using Fubu.Generation;
using FubuCore;
using FubuCsProjFile;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Generation
{
    [TestFixture]
    public class AddCommand_and_AddInput_Tester
    {
        [Test]
        public void input_can_determine_the_short_name_if_none_is_supplied()
        {
            new AddInput {ProjectName = "MyCompany.AwesomeSauce"}
                .DetermineShortName()
                .ShouldEqual("AwesomeSauce");

            new AddInput { ProjectName = "MyCompany.Core.Consumers" }
                .DetermineShortName()
                .ShouldEqual("CoreConsumers");
        }

        [Test]
        public void input_uses_short_name_flag_if_it_exists()
        {
            new AddInput
            {
                ProjectName = "MyCompany.AwesomeSauce",
                ShortNameFlag = "Radical"
            }.DetermineShortName().ShouldEqual("Radical");
        }

        [Test]
        public void adds_a_test_project_by_default()
        {
            var input = new AddInput
            {
                Profile = "library",
                ProjectName = "Foo"
            };

            var request = AddCommand.BuildTemplateRequest(input, "MyFoo.sln");

            request.TestingProjects.Single()
                .Name.ShouldEqual("Foo.Testing");
        }

        [Test]
        public void sets_the_dot_net_version()
        {
            var input = new AddInput
            {
                Profile = "library",
                ProjectName = "Foo",
                DotNetFlag = DotNetVersion.V45
            };

            var request = AddCommand.BuildTemplateRequest(input, "MyFoo.sln");
            request.Projects.First().Version.ShouldEqual(DotNetVersion.V45);
            request.TestingProjects.First().Version.ShouldEqual(DotNetVersion.V45);
        }

        [Test]
        public void sets_the_dot_net_version_2()
        {
            var input = new AddInput
            {
                Profile = "library",
                ProjectName = "Foo",
                DotNetFlag = DotNetVersion.V40
            };

            var request = AddCommand.BuildTemplateRequest(input, "MyFoo.sln");
            request.Projects.First().Version.ShouldEqual(DotNetVersion.V40);
            request.TestingProjects.First().Version.ShouldEqual(DotNetVersion.V40);
        }

        [Test]
        public void no_tests_if_the_no_testing_flag_is_selected()
        {
            var input = new AddInput
            {
                Profile = "library",
                ProjectName = "Foo",
                NoTestsFlag = true
            };

            var request = AddCommand.BuildTemplateRequest(input, "MyFoo.sln");
            request.TestingProjects.Any()
                .ShouldBeFalse();
        }

        [Test]
        public void assert_failure_if_fubudocs_and_project_name_does_not_end_with_docs()
        {
            var input = new AddInput
            {
                Profile = "fubudocs",
                ProjectName = "Wrong"
            };

            Exception<ApplicationException>.ShouldBeThrownBy(() => {
                input.AssertValid();
            })
            .Message.ShouldContain("Any FubuDocs project must be named with the '.Docs' suffix");
        }

        [Test]
        public void assert_happy_path_with_fubudocs()
        {
            var input = new AddInput
            {
                Profile = "fubudocs",
                ProjectName = "Right.Docs"
            };

            input.AssertValid();
        }

        [Test]
        public void using_fubudocs_as_the_profile_sets_the_no_tests_flag_to_true()
        {
            var input = new AddInput
            {
                Profile = "fubudocs",
                ProjectName = "Right.Docs"
            };

            input.NoTestsFlag.ShouldBeTrue();
        }

        [Test]
        public void other_than_fubudocs_no_tests_flag_is_false()
        {
            new AddInput{Profile = "web-app"}.NoTestsFlag
                .ShouldBeFalse();
        }
    }
}