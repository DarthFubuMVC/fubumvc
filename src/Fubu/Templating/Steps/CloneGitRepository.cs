using System;
using System.IO;
using System.Reflection;
using FubuCore;
using FubuMVC.Core;

namespace Fubu.Templating.Steps
{
    public class CloneGitRepository : ITemplateStep
    {
        private readonly IProcessFactory _processFactory;
        private readonly IFileSystem _fileSystem;

        public CloneGitRepository(IProcessFactory processFactory, IFileSystem fileSystem)
        {
            _processFactory = processFactory;
            _fileSystem = fileSystem;
        }

        public string Describe(TemplatePlanContext context)
        {
            return "Clone git repository from {0}".ToFormat(context.Input.GitFlag);
        }

        public void Execute(TemplatePlanContext context)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var registry = _fileSystem.LoadFromFile<GitAliasRegistry>(dir, GitAliasRegistry.ALIAS_FILE);
            var url = context.Input.GitFlag;
            var token = registry.AliasFor(url);
            if(token != null)
            {
                url = token.Url;
            }

            var gitProcess = _processFactory
                .Create(p =>
                            {
                                p.UseShellExecute = false;
                                p.FileName = "git";
                                p.Arguments = "clone {0} {1}".ToFormat(url, context.TempDir);
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