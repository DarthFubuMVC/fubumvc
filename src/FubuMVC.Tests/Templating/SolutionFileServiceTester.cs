using System.Text;
using Fubu.Templating;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Templating
{
    [TestFixture]
    public class SolutionFileServiceTester
    {
        [Test]
        public void should_add_project_references()
        {
            // build it up through a stringbuilder to use the environment-specific newline
            var solutionBuilder = new StringBuilder("Microsoft Visual Studio Solution File, Format Version 11.00")
                .AppendLine()
                .AppendLine("# Visual Studio 2010")
                .AppendLine(@"Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""FubuMVC.StructureMap"", ""FubuMVC.StructureMap\FubuMVC.StructureMap.csproj"", ""{ABFEA520-820C-4B77-9015-6A09E24252FA}""")
                .AppendLine("EndProject")
                .AppendLine("Global")
                .AppendLine("	GlobalSection(SolutionConfigurationPlatforms) = preSolution")
                .AppendLine("		Debug|Any CPU = Debug|Any CPU")
                .AppendLine("		Release|Any CPU = Release|Any CPU")
                .AppendLine("	EndGlobalSection")
                .AppendLine("	GlobalSection(SolutionProperties) = preSolution")
                .AppendLine("		HideSolutionNode = FALSE")
                .AppendLine("	EndGlobalSection")
                .AppendLine("EndGlobal");

            var system = new FileSystem();
            var solutionFile = "tmp.sln";
            system.AppendStringToFile(solutionFile, solutionBuilder.ToString());

            var project = new CsProj
                              {
                                  Name = "Test",
                                  ProjectGuid = "123",
                                  RelativePath = @"example1\example1.csproj"
                              };
            var service = new SolutionFileService(system);
            service.AddProject(solutionFile, project);

            var solutionContents = system.ReadStringFromFile(solutionFile);
            var lines = service.SplitSolution(solutionContents);

            lines[4].ShouldEqual("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Test\", \"example1\\example1.csproj\", \"{123}\"");
            lines[5].ShouldEqual("EndProject");

            system.DeleteFile(solutionFile);
        }
    }
}