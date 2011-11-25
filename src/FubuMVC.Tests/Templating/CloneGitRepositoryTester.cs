using System;
using System.Diagnostics;
using Fubu;
using Fubu.Templating;
using Fubu.Templating.Steps;
using FubuCore;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Templating
{
    [TestFixture]
    public class CloneGitRepositoryTester : InteractionContext<CloneGitRepository>
    {
        private NewCommandInput _input;
        private TemplatePlanContext _context;

        protected override void beforeEach()
        {
            _input = new NewCommandInput();
            _context = new TemplatePlanContext
                           {
                               TargetPath = "Test",
                               Input = _input,
                               TempDir = Guid.NewGuid().ToString()
                           };
        }

        [Test]
        public void should_configure_and_start_git_process()
        {
            _input.GitFlag = "git://test.git";

            var info = new ProcessStartInfo();
            MockFor<IProcessFactory>()
                .Expect(f => f.Create(p => { }))
                .IgnoreArguments()
                .WhenCalled(mi =>
                                {
                                    var configure = (Action<ProcessStartInfo>) mi.Arguments[0];
                                    configure(info);
                                })
                .Return(MockFor<IProcess>());

            MockFor<IProcess>()
                .Expect(p => p.Start())
                .Return(true);

            MockFor<IProcess>()
                .Expect(p => p.WaitForExit());

            MockFor<IProcess>()
                .Expect(p => p.ExitCode)
                .Return(0);

            ClassUnderTest.Execute(_context);

            info.UseShellExecute.ShouldBeFalse();
            info.FileName.ShouldEqual("git");
            info.Arguments.ShouldEqual("clone {0} {1}".ToFormat(_input.GitFlag, _context.TempDir));

            VerifyCallsFor<IProcess>();
        }

        [Test]
        public void should_throw_fubu_exception_if_process_exit_code_is_not_zero()
        {
            MockFor<IProcessFactory>()
                .Expect(f => f.Create(null))
                .IgnoreArguments()
                .Return(MockFor<IProcess>());

            MockFor<IProcess>()
                .Expect(p => p.ExitCode)
                .Return(2);

            Exception<FubuException>.ShouldBeThrownBy(() => ClassUnderTest.Execute(_context));
        }
    }
}