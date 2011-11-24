using FubuCore;
using FubuMVC.Core;

namespace Fubu
{
    public class CloneGitRepositoryTemplateStep : ITemplateStep
    {
        private readonly IProcessFactory _processFactory;

        public CloneGitRepositoryTemplateStep(IProcessFactory processFactory)
        {
            _processFactory = processFactory;
        }

        public void Execute(TemplatePlanContext context)
        {
            var gitProcess = _processFactory
                .Create(p =>
                            {
                                p.UseShellExecute = false;
                                p.FileName = "git";
                                p.Arguments = "clone {0} {1}".ToFormat(context.Input.GitFlag, context.TempDir);
                            });

            gitProcess.Start();
            gitProcess.WaitForExit();
            if (gitProcess.ExitCode != 0)
            {
                throw new FubuException(gitProcess.ExitCode, "Command finished with a non-zero exit code");
            }
        }
    }
}