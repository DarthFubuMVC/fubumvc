using System;
using System.IO;
using Fubu;
using Fubu.Templating;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace fubu.Testing.Templating
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

            ClassUnderTest.Execute(_input, _plan, ctx => { });
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
        public void should_set_target_directory_to_project_name_if_output_is_not_specified_and_solution_is_not_specified()
        {
            execute();

            _context.TargetPath.ShouldEqual(Path.Combine(Environment.CurrentDirectory, _input.ProjectName));
        }

        [Test]
        public void should_set_target_directory_to_solution_directory_if_solution_is_specified_and_output_is_not()
        {
            _input.SolutionFlag = "src/Test.sln";
            MockFor<IFileSystem>()
                .Expect(s => s.GetDirectory(_input.SolutionFlag))
                .Return("src");

            execute();

            _context.TargetPath.ShouldEqual(Path.Combine(Environment.CurrentDirectory, "src"));
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

        [Test]
        public void should_register_error_for_an_exception_thrown_in_a_step()
        {
            var error = "123";
            _plan.AddStep(MockFor<ITemplateStep>());

            MockFor<ITemplateStep>()
                .Expect(s => s.Execute(null))
                .IgnoreArguments()
                .WhenCalled(mi =>
                                {
                                    _context = (TemplatePlanContext) mi.Arguments[0];
                                    throw new ArgumentException(error);
                                });

            ClassUnderTest.Execute(_input, _plan, ctx => { });

            _context
                .Errors
                .ShouldContain(error);
        }

        [Test]
        public void should_invoke_callback_on_context()
        {
            _plan.AddStep(MockFor<ITemplateStep>());

            MockFor<ITemplateStep>()
                .Expect(s => s.Execute(null))
                .IgnoreArguments()
                .WhenCalled(mi =>
                                {
                                    _context = (TemplatePlanContext) mi.Arguments[0];
                                });

            ClassUnderTest.Execute(_input, _plan, ctx => ctx.ShouldEqual(_context));
        }
    }
}