using FubuCore;

namespace Fubu.Templating.Steps
{
    public class AutoRunFubuRake : ITemplateStep
    {
        public static readonly string FubuRakeFile = ".fuburake";

        private readonly IFileSystem _fileSystem;
        private readonly IRakeRunner _rakeRunner;

        public AutoRunFubuRake(IFileSystem fileSystem, IRakeRunner rakeRunner)
        {
            _fileSystem = fileSystem;
            _rakeRunner = rakeRunner;
        }

        public string Describe(TemplatePlanContext context)
        {
            if(!shouldExecute(context))
            {
                return null;
            }

            return "rake {0}".ToFormat(getRakeFile(context));
        }

        private bool shouldExecute(TemplatePlanContext context)
        {
            return _fileSystem.FileExists(getRakeFile(context));
        }

        private string getRakeFile(TemplatePlanContext context)
        {
            return _fileSystem.GetFullPath(FileSystem.Combine(context.TempDir, FubuRakeFile));
        }

        public void Execute(TemplatePlanContext context)
        {
            if(shouldExecute(context))
            {
                _rakeRunner.Run(context, getRakeFile(context));
            }
        }
    }
}