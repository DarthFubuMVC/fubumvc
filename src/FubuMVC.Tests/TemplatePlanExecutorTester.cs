using System;
using System.IO;
using Fubu;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class TemplatePlanExecutorTester : InteractionContext<TemplatePlanExecutor>
    {
        private TemplatePlan _plan;
        private NewCommandInput _input;
        private TemplatePlanContext _context;

        protected override void beforeEach()
        {
            _plan = new TemplatePlan();
            _input = new NewCommandInput {ProjectName = "Test"};

            MockFor<IFileSystem>()
                .Expect(f => f.DirectoryExists(Arg<string>.Is.NotNull))
                .Return(false);
        }

        private void execute()
        {
            _plan.AddStep(MockFor<ITemplateStep>());

            MockFor<ITemplateStep>()
                .Expect(s => s.Execute(null))
                .IgnoreArguments()
                .WhenCalled(mi =>
                                {
                                    _context = (TemplatePlanContext) mi.Arguments[0];
                                });

            ClassUnderTest.Execute(_input, _plan);
        }

        [Test]
        public void should_set_input_on_context()
        {
            execute();
            _context.Input.ShouldEqual(_input);
        }

        [Test]
        public void should_set_and_create_temp_dir()
        {
            var tmpDir = "";
            MockFor<IFileSystem>()
                .Expect(f => f.CreateDirectory(Arg<string>.Is.NotNull))
                .WhenCalled(mi =>
                                {
                                    tmpDir = (string) mi.Arguments[0];
                                });

            execute();

            VerifyCallsFor<IFileSystem>();
            _context.TempDir.IsEmpty().ShouldBeFalse();
            _context.TempDir.ShouldEqual(tmpDir);
        }

        [Test]
        public void should_set_target_directory_to_project_name_if_output_is_not_specified()
        {
            execute();

            _context.TargetPath.ShouldEqual(Path.Combine(Environment.CurrentDirectory, _input.ProjectName));
        }

        [Test]
        public void should_set_target_directory_to_output_if_output_is_specified()
        {
            _input.OutputFlag = "123";
            execute();

            _context.TargetPath.ShouldEqual(Path.Combine(Environment.CurrentDirectory, _input.OutputFlag));
        }

        [Test]
        public void should_remove_temp_directory()
        {
            var tmpDir = "";
            MockFor<IFileSystem>()
                .Expect(f => f.DeleteDirectory(Arg<string>.Is.NotNull))
                .WhenCalled(mi =>
                                {
                                    tmpDir = (string) mi.Arguments[0];
                                });

            execute();

            VerifyCallsFor<IFileSystem>();
            _context.TempDir.ShouldEqual(tmpDir);
        }

        [Test]
        public void should_execute_steps()
        {
            execute();
            VerifyCallsFor<ITemplateStep>();
        }
    }
}