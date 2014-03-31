using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Fubu.Generation;
using FubuCore;
using FubuCsProjFile;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks.Interfaces;

namespace fubu.Testing.Generation
{
    [TestFixture]
    public class when_generating_an_mvc_triad
    {
        private CsProjFile theProject;
        private string theCurrentDirectory;

        [SetUp]
        public void SetUp()
        {
            var theOriginalLocation = Environment.CurrentDirectory;

            var project = CsProjFile.CreateAtSolutionDirectory("MyProject", "MyProject");
            project.Save();

            var projectPath = project.FileName.ToFullPath();

            var projectFolder = project.FileName.ParentDirectory();
            theCurrentDirectory = projectFolder.AppendPath("Suite", "Tools").ToFullPath();

            new FileSystem().CreateDirectory(theCurrentDirectory);
            Environment.CurrentDirectory = theCurrentDirectory;


            var input = new ViewInput
            {
                Name = "MyView",
                TemplateFlag = "view.spark",
                UrlFlag = "foo/bar"
            };

            try
            {
                MvcBuilder.BuildView(input);

                theProject = CsProjFile.LoadFrom(projectPath);
            }
            finally
            {
                Environment.CurrentDirectory = theOriginalLocation;
            }
        }

        [Test]
        public void should_write_the_view_file()
        {
            File.Exists(theCurrentDirectory.AppendPath("MyView.spark"))
                .ShouldBeTrue();
        }

        [Test]
        public void should_write_the_input_model_file()
        {
            File.Exists(theCurrentDirectory.AppendPath("MyView.cs"))
                .ShouldBeTrue();
        }

        [Test]
        public void should_attach_the_view_to_the_csproj()
        {
            theProject.Find<Content>("Suite/Tools/MyView.spark".Replace('/', Path.DirectorySeparatorChar))
                .ShouldNotBeNull();
        }

        [Test]
        public void should_attach_the_code_file_to_the_csproj()
        {
            theProject.Find<CodeFile>("Suite/Tools/MyView.cs".Replace('/', Path.DirectorySeparatorChar))
                .ShouldNotBeNull();
        }

        [Test]
        public void uses_the_right_namespace_for_the_model()
        {
            new FileSystem().ReadStringFromFile(theCurrentDirectory.AppendPath("MyView.spark"))
                .ShouldContain("MyProject.Suite.Tools.MyView");
        }

        [Test]
        public void code_file_has_the_url()
        {
            new FileSystem().ReadStringFromFile(theCurrentDirectory.AppendPath("MyView.cs"))
                .ShouldContain("[UrlPattern(\"foo/bar\")]");

        }

    }
}