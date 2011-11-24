using System.Linq;
using Bottles.Zipping;
using Fubu;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class NewCommandTester : InteractionContext<NewCommand>
    {
        private TemplatePlan _plan;
        private NewCommandInput _input;

        protected override void beforeEach()
        {
            _input = new NewCommandInput
                         {
                             ProjectName = "Test"
                         };
            ClassUnderTest.FileSystem = MockFor<IFileSystem>();
            ClassUnderTest.ZipService = MockFor<IZipFileService>();
            ClassUnderTest.KeywordReplacer = MockFor<IKeywordReplacer>();
            ClassUnderTest.ProcessFactory = MockFor<IProcessFactory>();
            ClassUnderTest.PlanExecutor = MockFor<ITemplatePlanExecutor>();
        }

        private void executeCommand()
        {
            MockFor<ITemplatePlanExecutor>()
                .Expect(e => e.Execute(null, null))
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
                .ShouldContain(s => s.GetType() == typeof(UnzipTemplateStep));

            _plan
                .Steps
                .OfType<CloneGitRepositoryTemplateStep>()
                .ShouldHaveCount(0);
        }

        [Test]
        public void should_add_clone_git_repository_step_if_using_git()
        {
            _input.GitFlag = "git://test.git";

            executeCommand();

            _plan
                .Steps
                .OfType<CloneGitRepositoryTemplateStep>()
                .ShouldHaveCount(1);

            _plan
                .Steps
                .OfType<UnzipTemplateStep>()
                .ShouldHaveCount(0);
        }

        [Test]
        public void should_add_content_replacement_step()
        {
            executeCommand();

            _plan
                .Steps
                .Last()
                .ShouldBeOfType<ContentReplacerTemplateStep>();
        }
    }
}