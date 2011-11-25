using System.Collections.Generic;
using System.IO;
using Fubu;
using Fubu.Templating;
using Fubu.Templating.Steps;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Templating
{
    [TestFixture]
    public class ModifySolutionTester : InteractionContext<ModifySolution>
    {
        [Test]
        public void should_add_each_project_found()
        {
            var solutionFile = FileSystem.Combine("Templating", "data", "myproject.txt");
            var solutionDir = Path.GetDirectoryName(Path.GetFullPath(solutionFile));

            var projects = new List<CsProj>();
            for(var i = 0; i < 5; i++)
            {
                projects.Add(new CsProj());
            }

            MockFor<ICsProjGatherer>()
                .Expect(g => g.GatherProjects(solutionDir))
                .Return(projects);

            projects.Each(project => MockFor<ISolutionFileService>()
                                         .Expect(s => s.AddProject(solutionFile, project)));

            var input = new NewCommandInput {SolutionFlag = solutionFile};
            var context = new TemplatePlanContext {Input = input};

            ClassUnderTest.Execute(context);

            VerifyCallsFor<ISolutionFileService>();
        }
    }
}