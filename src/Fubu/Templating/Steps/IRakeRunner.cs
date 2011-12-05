using System;
using System.IO;
using System.Text;
using FubuCore;
using FubuMVC.Core;

namespace Fubu.Templating.Steps
{
    public interface IRakeRunner
    {
        void Run(TemplatePlanContext context, string rakeFile);
    }

    public class RakeRunner : IRakeRunner
    {
        private readonly IProcessFactory _processFactory;
        private readonly IFileSystem _fileSystem;

        public RakeRunner(IProcessFactory processFactory, IFileSystem fileSystem)
        {
            _processFactory = processFactory;
            _fileSystem = fileSystem;
        }

        public void Run(TemplatePlanContext context, string rakeFile)
        {
            var tempFile = FileSystem.Combine(context.TempDir, "{0}.rb".ToFormat(Guid.NewGuid()));
            using (var runner = GetType().Assembly.GetManifestResourceStream(GetType(), "rakerunner.rb"))
            {
                using (var fileStream = File.Create(tempFile))
                {
                    runner.CopyTo(fileStream);
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.WriteLine("FUBU_PROJECT_NAME = \"{0}\"".ToFormat(context.Input.ProjectName));
                        writer.WriteLine("Rake.application.run");
                        writer.Close();
                    }
                }
            }

            var rakeProcess = _processFactory
                .Create(s =>
                            {
                                s.FileName = "ruby";
                                s.UseShellExecute = false;
                                s.WorkingDirectory = context.TargetPath;
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