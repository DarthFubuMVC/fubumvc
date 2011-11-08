using System.Diagnostics;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using Serenity.Jasmine;
using FubuTestingSupport;
using System.Linq;
using System.Collections.Generic;

namespace Serenity.Testing.Jasmine
{
    [TestFixture]
    public class SpecificationGraphIntegratedTester
    {
        private AssetPipeline thePipeline;
        private SpecificationGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            thePipeline = new AssetPipeline();
            var builder = new AssetPipelineBuilder(new FileSystem(), thePipeline, new PackageLog());
            builder.LoadFiles(new PackageAssetDirectory{
                Directory = FileSystem.Combine("..", "..", "Jasmine", "TestPackage2"),
                PackageName = "Pak2"
            });

            theGraph = new SpecificationGraph(thePipeline);
        }

        [Test]
        public void specifications_have_the_correct_spec_path()
        {
            theGraph.FindSpecByLibraryName("spec6.js").Path().FullName.ShouldEqual("Pak2/folder1/folder2/spec6.js");
            theGraph.FindSpecByLibraryName("spec4.js").Path().FullName.ShouldEqual("Pak2/folder1/spec4.js");
            theGraph.FindSpecByLibraryName("spec1.js").Path().FullName.ShouldEqual("Pak2/spec1.js");
        }

        [Test]
        public void has_all_the_nodes()
        {
            var expectedValues =
                @"
Pak2
Pak2/folder1
Pak2/folder1/folder2
Pak2/folder1/folder2/spec6.js
Pak2/folder1/folder2/spec7.js
Pak2/folder1/folder2/spec8.js
Pak2/folder1/spec4.js
Pak2/folder1/spec5.js
Pak2/spec1.js
Pak2/spec2.js
Pak2/spec3.js
"
                    .ReadLines().Where(x => x.IsNotEmpty());


            theGraph.AllNodes.Select(x => x.FullName).ShouldHaveTheSameElementsAs(expectedValues);
        }


        [Test]
        public void find_specs_by_path()
        {
            ISpecNode findSpecs = theGraph.FindSpecNode(new SpecPath("Pak2/folder1/folder2"));
            findSpecs.ShouldNotBeNull();


            findSpecs.AllSpecifications.Select(x => x.LibraryName).ShouldHaveTheSameElementsAs("spec6.js", "spec7.js", "spec8.js");
            theGraph.FindSpecNode(new SpecPath("Pak2/folder1")).AllSpecifications.Select(x => x.LibraryName).ShouldHaveTheSameElementsAs("spec6.js", "spec7.js", "spec8.js", "spec4.js", "spec5.js");
            theGraph.FindSpecNode(new SpecPath("Pak2")).AllSpecifications.Select(x => x.LibraryName).ShouldHaveTheSameElementsAs("spec6.js", "spec7.js", "spec8.js", "spec4.js", "spec5.js", "spec1.js", "spec2.js", "spec3.js");
        }
    }
}