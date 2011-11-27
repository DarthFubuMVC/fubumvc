using System;
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
            var solutionContents = @"Microsoft Visual Studio Solution File, Format Version 11.00
# Visual Studio 2010
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""FubuMVC.StructureMap"", ""FubuMVC.StructureMap\FubuMVC.StructureMap.csproj"", ""{ABFEA520-820C-4B77-9015-6A09E24252FA}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal";
            var solutionFile = "tmp.sln";
            var system = new FileSystem();
            system.AppendStringToFile(solutionFile, solutionContents);

            var project = new CsProj
                              {
                                  Name = "Test",
                                  ProjectGuid = "123",
                                  RelativePath = @"example1\example1.csproj"
                              };
            var service = new SolutionFileService(system);
            service.AddProject(solutionFile, project);

            solutionContents = system.ReadStringFromFile(solutionFile);
            var lines = service.SplitSolution(solutionContents);

            lines[4].ShouldEqual("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Test\", \"example1\\example1.csproj\", \"{123}\"");
            lines[5].ShouldEqual("EndProject");

            system.DeleteFile(solutionFile);
        }
    }
}