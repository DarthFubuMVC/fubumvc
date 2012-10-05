using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetTagPlannerTester : InteractionContext<AssetTagPlanner>
    {
        [Test]
        public void find_subjects_where_some_can_be_found_and_some_cannot()
        {
            var pipeline = MockFor<IAssetFileGraph>();
            var file1 = new AssetFile("a.js");
            var file2 = new AssetFile("b.js");
            var file3 = new AssetFile("c.js");

            pipeline.Stub(x => x.Find(file1.Name)).Return(file1);
            pipeline.Stub(x => x.Find(file2.Name)).Return(file2);
            pipeline.Stub(x => x.Find(file3.Name)).Return(file3);

            var list = ClassUnderTest.FindSubjects(new string[]{"a.js", "missing.js", "b.js", "wrong.js", "c.js"}).ToList();

            list[0].ShouldBeTheSameAs(file1);
            list[1].ShouldBeOfType<MissingAssetTagSubject>().Name.ShouldEqual("missing.js");
            list[2].ShouldBeTheSameAs(file2);
            list[3].ShouldBeOfType<MissingAssetTagSubject>().Name.ShouldEqual("wrong.js");
            list[4].ShouldBeTheSameAs(file3);


        }


    }
}