using System;
using System.Linq;
using Fubu.Generation;
using FubuCore;
using FubuCsProjFile.Templating.Graph;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Generation
{
    [TestFixture]
    public class NewCommandTester
    {
        [Test]
        public void assert_folder_blows_up_when_directory_is_not_empty()
        {
            new FileSystem().CreateDirectory("not-empty");
            new FileSystem().WriteStringToFile("not-empty".AppendPath("foo.txt"), "anything");

            Exception<InvalidOperationException>.ShouldBeThrownBy(() => { NewCommand.AssertEmpty("not-empty"); });
        }

        [Test]
        public void assert_folder_is_empty_does_not_blow_for_empty_target()
        {
            new FileSystem().DeleteDirectory("target");
            new FileSystem().CreateDirectory("target");

            NewCommand.AssertEmpty("target");
        }

        [Test]
        public void assert_folder_is_empty_is_fine_when_directory_does_not_exist()
        {
            new FileSystem().DeleteDirectory("nonexistent");
            NewCommand.AssertEmpty("nonexistent");
        }

        [Test]
        public void default_ripple_is_public_only()
        {
            var input = new NewCommandInput
            {
                SolutionName = "NewThing",
            };

            TemplateRequest request = input.CreateRequestForSolution();

            request.Templates.ShouldContain("public-ripple");
            request.Templates.ShouldNotContain("edge-ripple");
            request.Templates.ShouldNotContain("floating-ripple");
        }

        [Test]
        public void dot_git_files_are_ignored()
        {
            NewCommand.IsBaselineFile(".git/something")
                .ShouldBeTrue();
        }

        [Test]
        public void license_file_is_ignored()
        {
            NewCommand.IsBaselineFile("license.txt")
                .ShouldBeTrue();
        }

        [Test]
        public void no_project_if_profile_is_empty()
        {
            var input = new NewCommandInput
            {
                SolutionName = "NewThing",
                RippleFlag = FeedChoice.Edge,
                Profile = "empty"
            };

            TemplateRequest request = input.CreateRequestForSolution();

            request.Projects.Any().ShouldBeFalse();
        }

        [Test]
        public void readme_files_are_ignored()
        {
            NewCommand.IsBaselineFile("readme").ShouldBeTrue();
            NewCommand.IsBaselineFile("readme.txt").ShouldBeTrue();
            NewCommand.IsBaselineFile("README.txt").ShouldBeTrue();
            NewCommand.IsBaselineFile("README.markdown").ShouldBeTrue();
            NewCommand.IsBaselineFile("README.md").ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_an_app_is_requested_within_the_new_request
    {
        [SetUp]
        public void SetUp()
        {
            var input = new NewCommandInput
            {
                SolutionName = "NewThing",
                Profile = "web-app",
            };

            TemplateRequest request = input.CreateRequestForSolution();

            project = request.Projects.Single();
        }

        private ProjectRequest project;

        [Test]
        public void is_a_project()
        {
            project.ShouldNotBeNull();
        }

        [Test]
        public void project_has_the_same_name_as_the_solution()
        {
            project.Name.ShouldEqual("NewThing");
        }
    }
}