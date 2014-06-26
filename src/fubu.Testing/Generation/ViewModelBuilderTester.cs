using System.Diagnostics;
using System.IO;
using Fubu.Generation;
using FubuCore;
using FubuCsProjFile;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace fubu.Testing.Generation
{
    [TestFixture]
    public class when_building_view_model_without_url   
    {
        private Location theLocation;
        private ViewInput theViewInput;
        private CodeFile theFile;
        private string expectedPath = "MyLib".AppendPath("MyLib", "A", "B", "MyModel.cs");

        [SetUp]
        public void SetUp()
        {
            new FileSystem().DeleteDirectory("MyLib");
            new FileSystem().CreateDirectory("MyLib");
            theLocation = new Location
            {
                Namespace = "MyLib.A.B",
                Project = CsProjFile.CreateAtSolutionDirectory("MyLib", "MyLib"),
                RelativePath = "A/B"
            };

            theViewInput = new ViewInput
            {
                Name = "MyModel",
                UrlFlag = null
            };

            theFile = ViewModelBuilder.BuildCodeFile(theViewInput, theLocation);


        }

        [Test]
        public void include()
        {
            theFile.Include.ShouldEqual("A\\B\\MyModel.cs");
        }

        [Test]
        public void builds_the_file()
        {
            
            File.Exists(expectedPath)
                .ShouldBeTrue();
        }

        private string[] theWrittenText()
        {
            return new FileSystem().ReadStringFromFile(expectedPath)
                                   .ReadLines().ToArray();
        }

        [Test]
        public void file_should_have_the_right_namespace()
        {
            theWrittenText().ShouldContain("namespace MyLib.A.B");
        }

        [Test]
        public void file_should_have_the_clss_name()
        {
            theWrittenText().Any(x => x.Contains("public class MyModel"))
                .ShouldBeTrue();
        }

        [Test]
        public void does_not_write_the_url()
        {
            theWrittenText().Any(x => x.Contains("UrlPattern"))
                .ShouldBeFalse();
        }
    }



    [TestFixture]
    public class when_building_view_model_with_url
    {
        private Location theLocation;
        private ViewInput theViewInput;
        private CodeFile theFile;
        private string expectedPath = "MyLib".AppendPath("MyLib", "A", "B", "MyModel.cs");

        [SetUp]
        public void SetUp()
        {
            new FileSystem().DeleteDirectory("MyLib");
            new FileSystem().CreateDirectory("MyLib");
            theLocation = new Location
            {
                Namespace = "MyLib.A.B",
                Project = CsProjFile.CreateAtSolutionDirectory("MyLib", "MyLib"),
                RelativePath = "A/B"
            };

            theViewInput = new ViewInput
            {
                Name = "MyModel",
                UrlFlag = "foo/bar/model"
            };

            theFile = ViewModelBuilder.BuildCodeFile(theViewInput, theLocation);


        }

        [Test]
        public void include()
        {
            theFile.Include.ShouldEqual("A\\B\\MyModel.cs");
        }

        [Test]
        public void builds_the_file()
        {

            File.Exists(expectedPath)
                .ShouldBeTrue();
        }

        private string[] theWrittenText()
        {
            return new FileSystem().ReadStringFromFile(expectedPath)
                                   .ReadLines().ToArray();
        }

        [Test]
        public void file_should_have_the_right_namespace()
        {
            theWrittenText().ShouldContain("namespace MyLib.A.B");
        }

        [Test]
        public void file_should_have_the_clss_name()
        {
            theWrittenText().Any(x => x.Contains("public class MyModel"))
                .ShouldBeTrue();
        }

        [Test]
        public void does_not_write_the_url()
        {
            theWrittenText().Any(x => x.Contains("[UrlPattern(\"foo/bar/model\")]"))
                .ShouldBeTrue();
        }
    }


}