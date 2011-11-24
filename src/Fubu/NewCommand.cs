using System;
using Bottles.Zipping;
using FubuCore;
using FubuCore.CommandLine;

namespace Fubu
{
    [CommandDescription("Creates a new FubuMVC solution", Name = "new")]
    public class NewCommand : FubuCommand<NewCommandInput>
    {
        public NewCommand()
        {
            FileSystem = new FileSystem();
            ZipService = new ZipFileService(FileSystem);
            KeywordReplacer = new KeywordReplacer();
            ProcessFactory = new ProcessFactory();
            PlanExecutor = new TemplatePlanExecutor(FileSystem);
            SolutionFileService = new SolutionFileService(FileSystem);
            CsProjGatherer = new CsProjGatherer(FileSystem);
        }

        public IFileSystem FileSystem { get; set; }
        public IZipFileService ZipService { get; set; }
        public IKeywordReplacer KeywordReplacer { get; set; }
        public IProcessFactory ProcessFactory { get; set; }
        public ITemplatePlanExecutor PlanExecutor { get; set; }
        public ISolutionFileService SolutionFileService { get; set; }
        public ICsProjGatherer CsProjGatherer { get; set; }

        public override bool Execute(NewCommandInput input)
        {
            var plan = new TemplatePlan();
            var findContentStep = input.GitFlag.IsNotEmpty()
                                      ? (ITemplateStep) new CloneGitRepositoryTemplateStep(ProcessFactory)
                                      : new UnzipTemplateStep(ZipService);
            
            plan.AddStep(findContentStep);
            plan.AddStep(new ContentReplacerTemplateStep(KeywordReplacer, FileSystem));
            
            if(input.SolutionFlag.IsNotEmpty())
            {
                plan.AddStep(new SolutionModifierTemplateStep(SolutionFileService, CsProjGatherer));
            }
            
            PlanExecutor.Execute(input, plan);
            Console.WriteLine("Solution {0} created", input.ProjectName);
            return true;
        }
    }
}
