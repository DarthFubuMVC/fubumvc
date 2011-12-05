using FubuCore;

namespace Fubu.Templating.Steps
{
    public class RunRakeFile : ITemplateStep
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRakeRunner _rakeRunner;

        public RunRakeFile(IFileSystem fileSystem, IRakeRunner rakeRunner)
        {
            _fileSystem = fileSystem;
            _rakeRunner = rakeRunner;
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
            _rakeRunner.Run(context, getRakeFile(context));
        }
    }
}