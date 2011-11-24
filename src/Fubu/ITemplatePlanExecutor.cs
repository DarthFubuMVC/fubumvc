using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace Fubu
{
    public interface ITemplatePlanExecutor
    {
        void Execute(NewCommandInput input, TemplatePlan plan);
    }

    public class TemplatePlanExecutor : ITemplatePlanExecutor
    {
        private readonly IFileSystem _fileSystem;

        public TemplatePlanExecutor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Execute(NewCommandInput input, TemplatePlan plan)
        {
            var context = new TemplatePlanContext
                              {
                                  Input = input,
                                  TempDir = createTempDir()
                              };

            _fileSystem.CreateDirectory(context.TempDir);

            var targetPath = input.OutputFlag.IsEmpty()
                                 ? input.ProjectName
                                 : input.OutputFlag;
            context.TargetPath = Path.Combine(Environment.CurrentDirectory, targetPath);

            plan
                .Steps
                .Each(step => step.Execute(context));

            _fileSystem.DeleteDirectory(context.TempDir);
        }

        // just a sanity check
        private string createTempDir()
        {
            var name = Guid.NewGuid().ToString();
            var tmpDir = Path.Combine(Environment.CurrentDirectory, name);
            if(_fileSystem.DirectoryExists(tmpDir))
            {
                return createTempDir();
            }

            return tmpDir;
        }
    }
}