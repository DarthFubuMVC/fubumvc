using System;
using System.IO;
using System.Text;
using FubuCore;
using FubuMVC.Core;

namespace Fubu.Templating.Steps
{
    public class RunRakeFile : ITemplateStep
    {
        private readonly IProcessFactory _processFactory;
        private readonly IFileSystem _fileSystem;

        public RunRakeFile(IProcessFactory processFactory, IFileSystem fileSystem)
        {
            _processFactory = processFactory;
            _fileSystem = fileSystem;
        }

        public string Describe(TemplatePlanContext context)
        {
            return "rake {0}".ToFormat(getRakeFile(context));
        }

        private string getRakeFile(TemplatePlanContext context)
        {
            return _fileSystem.GetFullPath(FileSystem.Combine(context.TempDir, context.Input.RakeFlag));
        }

        public void Execute(TemplatePlanContext context)
        {
            var tempFile = FileSystem.Combine(context.TempDir, "{0}.rb".ToFormat(Guid.NewGuid()));
            using (var runner = GetType().Assembly.GetManifestResourceStream(GetType(), "rakerunner.rb"))
            {
                using (var stream = File.Create(tempFile))
                {
                    runner.CopyTo(stream);
                    stream.Close();
                }
            }

            var rakeFile = getRakeFile(context);
            var rakeProcess = _processFactory
                .Create(s =>
                            {
                                s.FileName = "ruby";
                                s.UseShellExecute = false;
                                s.WorkingDirectory = context.TargetPath;
                                s.EnvironmentVariables.Add("FUBUPROJECTNAME", context.Input.ProjectName);
                                s.Arguments = "{0} --rakefile {1}".ToFormat(tempFile, rakeFile);
                            });

            rakeProcess.Start();
            rakeProcess.WaitForExit();

            _fileSystem.DeleteFile(tempFile);

            if (rakeProcess.ExitCode != 0)
            {
                var message = new StringBuilder()
                    .AppendLine("Command finished with a non-zero exit code")
                    .AppendLine(rakeProcess.GetErrors());
                throw new FubuException(rakeProcess.ExitCode, message.ToString());
            }
        }
    }
}