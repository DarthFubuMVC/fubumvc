using System.Linq;
using Bottles.Zipping;
using Fubu;
using Fubu.Templating;
using Fubu.Templating.Steps;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace fubu.Testing.Templating
{
    [TestFixture]
    public class NewCommandTester : InteractionContext<NewCommand>
    {
        private TemplatePlan _plan;
        private NewCommandInput _input;

        protected override void beforeEach()
        {
            _input = new NewCommandInput { ProjectName = "Test" };

            ClassUnderTest.FileSystem = MockFor<IFileSystem>();
            ClassUnderTest.ZipService = MockFor<IZipFileService>();
            ClassUnderTest.KeywordReplacer = MockFor<IKeywordReplacer>();
            ClassUnderTest.ProcessFactory = MockFor<IProcessFactory>();
            ClassUnderTest.PlanExecutor = MockFor<ITemplatePlanExecutor>();
        }

        private void executeCommand()
        {
            MockFor<ITemplatePlanExecutor>()
                .Expect(e => e.Execute(null, null, null))
                .IgnoreArguments()
                .WhenCalled(mi =>
                                {
                                    _plan = (TemplatePlan) mi.Arguments[1];
                                });

            ClassUnderTest.Execute(_input);
        }

        [Test]
        public void should_add_unzip_step_if_not_using_git()
        {
            executeCommand();

            _plan
                .Steps
                .ShouldContain(s => s.GetType() == typeof(UnzipTemplate));

            _plan
                .Steps
                .OfType<CloneGitRepository>()
                .ShouldHaveCount(0);
        }

        [Test]
        public void should_add_clone_git_repository_step_if_using_git()
        {
            _input.GitFlag = "git://test.git";

            executeCommand();

            _plan
                .Steps
                .OfType<CloneGitRepository>()
                .ShouldHaveCount(1);

            _plan
                .Steps
                .OfType<UnzipTemplate>()
                .ShouldHaveCount(0);
        }

        [Test]
        public void should_add_replace_keywords_step()
        {
            executeCommand();

            _plan
                .Steps
                .OfType<ReplaceKeywords>()
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_add_move_content_step()
        {
            executeCommand();

            _plan
                .Steps
                .OfType<MoveContent>()
                .ShouldHaveCount(1);
        }
    }
}
