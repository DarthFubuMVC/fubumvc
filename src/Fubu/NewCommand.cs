using System;
using System.Collections.Generic;
using System.Linq;
using Bottles.Zipping;
using Fubu.Templating;
using Fubu.Templating.Steps;
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
            RakeRunner = new RakeRunner(ProcessFactory, FileSystem);
        }

        public IFileSystem FileSystem { get; set; }
        public IZipFileService ZipService { get; set; }
        public IKeywordReplacer KeywordReplacer { get; set; }
        public IProcessFactory ProcessFactory { get; set; }
        public ITemplatePlanExecutor PlanExecutor { get; set; }
        public ISolutionFileService SolutionFileService { get; set; }
        public ICsProjGatherer CsProjGatherer { get; set; }
        public IRakeRunner RakeRunner { get; set; }

        public override bool Execute(NewCommandInput input)
        {
            var plan = new TemplatePlan();
            var findContentStep = input.GitFlag.IsNotEmpty()
                                      ? (ITemplateStep) new CloneGitRepository(ProcessFactory, FileSystem)
                                      : new UnzipTemplate(ZipService);
            
            plan.AddStep(findContentStep);
            plan.AddStep(new ReplaceKeywords(KeywordReplacer, FileSystem));
            
            if(input.SolutionFlag.IsNotEmpty())
            {
                plan.AddStep(new ModifySolution(SolutionFileService, CsProjGatherer));
            }

            plan.AddStep(new MoveContent(FileSystem));

            if (input.RakeFlag.IsNotEmpty())
            {
                plan.AddStep(new RunRakeFile(FileSystem, RakeRunner));
            }

            plan.AddStep(new AutoRunFubuRake(FileSystem, RakeRunner));
            plan.AddStep(new RemoveTemporaryContent());

            var hasErrors = false;
            PlanExecutor.Execute(input, plan, ctx =>
                                                  {
                                                      Console.ForegroundColor = ConsoleColor.Red;
                                                      ctx.Errors.Each(error => Console.WriteLine(error));
                                                      Console.ForegroundColor = ConsoleColor.White;
                                                      hasErrors = ctx.Errors.Any();
                                                  });

            if (hasErrors)
            {
                return false;
            }

            Console.WriteLine("Solution {0} created", input.ProjectName);
            return true;
        }
    }
}
