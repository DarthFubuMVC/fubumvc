using Fubu.Generation;
using Fubu.Generation.Templates;
using FubuCore;
using FubuCsProjFile;
using FubuCsProjFile.Templating.Graph;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Generation.Templates
{
    [TestFixture]
    public class FileTemplateTester
    {
        [Test]
        public void create_from_embedded_resource()
        {
            var template = FileTemplate.Embedded("view.spark");
            template.RawText.ShouldContain("<viewdata model=\"%MODEL%\" />");
            template.Extension.ShouldEqual(".spark");
        }

        [Test]
        public void load_from_file()
        {
            new FileSystem().WriteStringToFile("foo.spark", "<h1>Rock on!</h1>");

            var template = FileTemplate.FromFile("foo.spark");

            template.RawText.ShouldEqual("<h1>Rock on!</h1>");
            template.Extension.ShouldEqual(".spark");
        }

        [Test]
        public void get_content()
        {
            var substitutions = new Substitutions();
            substitutions.Set("%MODEL%", "Foo.Bar");

            var template = FileTemplate.Embedded("view.spark");
            template.Contents(substitutions).ShouldContain("<viewdata model=\"Foo.Bar\" />");
        }

        
    }

    [TestFixture]
    public class when_finding_a_file_template
    {
        private Location theLocation;

        [SetUp]
        public void SetUp()
        {
            new FileSystem().DeleteDirectory("src");

            var project = CsProjFile.CreateAtSolutionDirectory("Foo", "src");
            project.Save();

            theLocation = new Location
            {
                Project = project
            };
        }

        [Test]
        public void fall_all_the_way_through_to_the_embedded_resource_if_nothing_else_exists()
        {
            var template = FileTemplate.Find(theLocation, "view.spark");

            template.RawText.ShouldEqual(FileTemplate.Embedded("view.spark").RawText);
        }

        [Test]
        public void use_the_solution_template_if_it_exists()
        {
            new FileSystem().WriteStringToFile(theLocation.SrcFolder().AppendPath("templates","view.spark"), "The solution template!");

            FileTemplate.Find(theLocation, "view.spark")
            .RawText.ShouldContain("The solution template!");
        }

        [Test]
        public void choose_the_project_template_if_it_exists_and_takes_precedence_over_the_solution()
        {
            new FileSystem().WriteStringToFile(theLocation.SrcFolder().AppendPath("templates", "view.spark"), "The solution template!");


            new FileSystem().WriteStringToFile(theLocation.ProjectFolder().AppendPath("view.spark"), "The project template!");

            FileTemplate.Find(theLocation, "view.spark")
                .RawText.ShouldContain("The project template!");
        }
    }
}