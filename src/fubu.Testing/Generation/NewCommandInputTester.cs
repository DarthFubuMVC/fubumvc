using System;
using System.Linq;
using Fubu.Generation;
using FubuCsProjFile;
using FubuCsProjFile.Templating.Graph;
using FubuCsProjFile.Templating.Runtime;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace fubu.Testing.Generation
{
    [TestFixture]
    public class NewCommandInputTester
    {
        [Test]
        public void solution_path_is_just_current_plus_solution_name_by_default()
        {
            var input = new NewCommandInput
            {
                SolutionName = "MySolution"
            };

            input.SolutionDirectory().ToFullPath()
                 .ShouldEqual(Environment.CurrentDirectory.AppendPath("MySolution").ToFullPath());
        }

        [Test]
        public void public_only_is_the_default_ripple_setting()
        {
            new NewCommandInput().RippleFlag.ShouldEqual(FeedChoice.PublicOnly);
        }

        [Test]
        public void choose_the_public_ripple()
        {
            var input = new NewCommandInput
            {
                SolutionName = "NewThing",
                RippleFlag = FeedChoice.PublicOnly
            };

            var request = input.CreateRequestForSolution();

            request.Templates.ShouldContain("public-ripple");
            request.Templates.ShouldNotContain("edge-ripple");
            request.Templates.ShouldNotContain("floating-ripple");
        }

        [Test]
        public void choose_the_float_ripple()
        {
            var input = new NewCommandInput
            {
                SolutionName = "NewThing",
                RippleFlag = FeedChoice.FloatingEdge
            };

            var request = input.CreateRequestForSolution();

            request.Templates.ShouldNotContain("public-ripple");
            request.Templates.ShouldNotContain("edge-ripple");
            request.Templates.ShouldContain("floating-ripple");
        }

        [Test]
        public void choose_the_edge_ripple()
        {
            var input = new NewCommandInput
            {
                SolutionName = "NewThing",
                RippleFlag = FeedChoice.Edge
            };

            var request = input.CreateRequestForSolution();

            request.Templates.ShouldNotContain("public-ripple");
            request.Templates.ShouldContain("edge-ripple");
            request.Templates.ShouldNotContain("floating-ripple");
        }

        [Test]
        public void choose_the_edge_feed_by_flag()
        {
            var input = new NewCommandInput
            {
                EdgeFlag = true
            };

            input.RippleFlag.ShouldEqual(FeedChoice.Edge);
        }

        [Test]
        public void choose_the_floating_edge_feed_by_flag()
        {
            var input = new NewCommandInput
            {
                FloatingFlag = true
            };

            input.RippleFlag.ShouldEqual(FeedChoice.FloatingEdge);
        }

        [Test]
        public void sets_the_short_name_of_the_project_by_default()
        {
            var input = new NewCommandInput
            {
                SolutionName = "MyCompany.NewThing",
                RippleFlag = FeedChoice.Edge
            };

            var request = input.CreateRequestForSolution();

            request.Substitutions.ValueFor(ProjectPlan.SHORT_NAME)
                .ShouldEqual("NewThing");

        }

        [Test]
        public void sets_the_dot_net_version()
        {
            var input = new NewCommandInput
            {
                SolutionName = "MyCompany.NewThing",
                RippleFlag = FeedChoice.Edge,
                DotNetFlag = DotNetVersion.V45
            };

            var request = input.CreateRequestForSolution();

            request.Projects.First()
                .Version.ShouldEqual(input.DotNetFlag);

            request.TestingProjects.First()
                .Version.ShouldEqual(input.DotNetFlag);

        }

        [Test]
        public void sets_the_short_name_of_the_project_by_explicit_flag()
        {
            var input = new NewCommandInput
            {
                SolutionName = "MyCompany.NewThing",
                RippleFlag = FeedChoice.Edge,
                ShortNameFlag = "NewThang"
            };

            var request = input.CreateRequestForSolution();

            request.Substitutions.ValueFor(ProjectPlan.SHORT_NAME)
                .ShouldEqual("NewThang");

        }

        [Test]
        public void profile_is_empty_no_project_requests()
        {
            var input = new NewCommandInput
            {
                SolutionName = "MyCompany.NewThing",
                RippleFlag = FeedChoice.Edge,
                ShortNameFlag = "NewThang",
                Profile = "empty"
            };

            var request = input.CreateRequestForSolution();
            request.Projects.Any().ShouldBeFalse();
        }

        [Test]
        public void no_profile_adds_a_web_app_project_and_tests()
        {
            var input = new NewCommandInput
            {
                SolutionName = "MyCompany.NewThing",
                RippleFlag = FeedChoice.Edge,
                ShortNameFlag = "NewThang",
            };

            var request = input.CreateRequestForSolution();
            request.Projects.Any().ShouldBeTrue();
            request.TestingProjects.Any().ShouldBeTrue();
                
        }

        [Test]
        public void no_tests_if_no_tests_flag_is_selected()
        {
            var input = new NewCommandInput
            {
                SolutionName = "MyCompany.NewThing",
                RippleFlag = FeedChoice.Edge,
                ShortNameFlag = "NewThang",
                NoTestsFlag = true
            };

            var request = input.CreateRequestForSolution();
            request.Projects.Any().ShouldBeTrue();
            request.TestingProjects.Any().ShouldBeFalse();
                
        }

        [Test]
        public void to_template_choices_basics()
        {
            var input = new NewCommandInput
            {
                SolutionName = "MyCompany.NewThing",
                RippleFlag = FeedChoice.Edge,
                ShortNameFlag = "NewThang",
                NoTestsFlag = true,
                OptionsFlag = new string[]{"a", "b", "c"}
            };

            var choices = input.ToTemplateChoices();
            choices.Category.ShouldEqual("new");
            choices.ProjectName.ShouldEqual(input.SolutionName);
            choices.ProjectType.ShouldEqual(input.Profile);
            choices.Options.ShouldHaveTheSameElementsAs("a", "b", "c");

        }

        [Test]
        public void default_version_is_vs2012()
        {
            new NewCommandInput().VersionFlag.ShouldEqual("VS2012");
        }

        [Test]
        public void transfers_the_version_to_the_request()
        {
            new NewCommandInput
            {
                VersionFlag = Solution.VS2010,
                SolutionName = "Foo"
            }
                .CreateRequestForSolution()
                .Version.ShouldEqual(Solution.VS2010);

            new NewCommandInput
            {
                VersionFlag = Solution.VS2012,
                SolutionName = "Foo"
            }
                .CreateRequestForSolution()
                .Version.ShouldEqual(Solution.VS2012);

            new NewCommandInput
            {
                VersionFlag = Solution.VS2013,
                SolutionName = "Foo"
            }
                .CreateRequestForSolution()
                .Version.ShouldEqual(Solution.VS2013);
        }
    }
}