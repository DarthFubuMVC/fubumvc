using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class CombinationTester : InteractionContext<Core.Assets.Content.Combination>
    {
        private IContentSource[] theInners;
        private AssetFile[] files1;
        private AssetFile[] files2;
        private AssetFile[] files3;
        private AssetFile[] files4;
        private AssetFile[] allFiles;
        private string[] theInnerContent;
        private IContentPipeline thePipeline;

        protected override void beforeEach()
        {
            theInners = Services.CreateMockArrayFor<IContentSource>(4);

            files1 = new AssetFile[]{new AssetFile("a.js"), new AssetFile("b.js")};
            files2 = new AssetFile[]{new AssetFile("c.js"), new AssetFile("d.js")};
            files3 = new AssetFile[]{new AssetFile("e.js"), new AssetFile("f.js")};
            files4 = new AssetFile[]{new AssetFile("g.js"), new AssetFile("h.js")};

            allFiles = files1.Union(files2).Union(files3).Union(files4).ToArray();

            theInners[0].Stub(x => x.Files).Return(files1);
            theInners[1].Stub(x => x.Files).Return(files2);
            theInners[2].Stub(x => x.Files).Return(files3);
            theInners[3].Stub(x => x.Files).Return(files4);

            theInnerContent = new string[]{"1", "2", "3", "4"};

            thePipeline = MockFor<IContentPipeline>();

            for (int i = 0; i < theInners.Length; i++)
            {
                theInners[i].Stub(x => x.GetContent(thePipeline)).Return(theInnerContent[i]);
            }
        }

        [Test]
        public void inner_sources_returns_the_____wait_for_it______the_inner_sources()
        {
            ClassUnderTest.InnerSources.ShouldHaveTheSameElementsAs(theInners);
        }

        [Test]
        public void files_should_return_a_union_of_all_the_files_in_the_inner_sources()
        {
            ClassUnderTest.Files.ShouldHaveTheSameElementsAs(allFiles);
        }

        [Test]
        public void the_content_should_include_all_the_content_in_order_of_the_inners_and_javascript_delimiters()
        {
            var separator = Core.Assets.Content.Combination.Separator + ";;" + Environment.NewLine + Environment.NewLine;
            ClassUnderTest.GetContent(thePipeline).ShouldEqual(theInnerContent.Join(separator) + Environment.NewLine + Environment.NewLine + ";;");
        }

        [Test]
        public void to_string_because_it_matters_for_automated_testing()
        {
            ClassUnderTest.ToString().ShouldEqual("Combination");
        }
    }
}