using System;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace FubuMVC.Tests.Assets.Content
{





    [TestFixture]
    public class ContentPlannerTester : InteractionContext<ContentPlanner>
    {
        private AssetFile[] theFiles;

        [Test]
        public void throw_argument_out_of_range_exception_when_findfiles_cannot_find_anything()
        {
            MockFor<IAssetCombinationCache>().Stub(x => x.FindCombination("script1")).Return(null);
            MockFor<IAssetPipeline>().Stub(x => x.Find("script1")).Return(null);

            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.FindFiles("script1");
            });
        }

        [Test]
        public void accept_a_visitor_simple()
        {
            var mocks = new MockRepository();

            var visitor = mocks.StrictMock<IContentPlanVisitor>();


            theFiles = new AssetFile[]{
                new AssetFile("a.js"){FullPath = "a.js"}, 
                new AssetFile("b.js"){FullPath = "b.js"}, 
                new AssetFile("c.js"){FullPath = "c.js"}, 
                new AssetFile("d.js"){FullPath = "d.js"}, 
            };

            var plan = new ContentPlan("something", theFiles);
            var read0 = plan.AllSources.ElementAt(0);
            var read1 = plan.AllSources.ElementAt(1);
            var read2 = plan.AllSources.ElementAt(2);
            var read3 = plan.AllSources.ElementAt(3);

            var combo1 = plan.Combine(new IContentSource[]{read1, read2});
            var combo2 = plan.Combine(new IContentSource[]{read0, combo1, read3});

            using (mocks.Ordered())
            {
                visitor.Expect(x => x.Push(combo2));

                visitor.Expect(x => x.Push(read0));
                visitor.Expect(x => x.Pop());

                visitor.Expect(x => x.Push(combo1));

                visitor.Expect(x => x.Push(read1));
                visitor.Expect(x => x.Pop());

                visitor.Expect(x => x.Push(read2));
                visitor.Expect(x => x.Pop());

                visitor.Expect(x => x.Pop());


                visitor.Expect(x => x.Push(read3));
                visitor.Expect(x => x.Pop());

                visitor.Expect(x => x.Pop());

                
            }

            mocks.ReplayAll();

            plan.AcceptVisitor(visitor);


            mocks.VerifyAll();

            

            
        }
    }

    [TestFixture]
    public class when_finding_files_and_the_name_matches_a_combination : InteractionContext<ContentPlanner>
    {
        private AssetFile[] theFiles;
        private ScriptFileCombination theCombination;

        protected override void beforeEach()
        {
            theFiles = new AssetFile[]{
                new AssetFile("a.js"), 
                new AssetFile("b.js"), 
                new AssetFile("c.js"), 
                new AssetFile("d.js"), 
            };

            theCombination = new ScriptFileCombination("combo1", theFiles);
            MockFor<IAssetCombinationCache>().Stub(x => x.FindCombination(theCombination.Name))
                .Return(theCombination);


        }

        [Test]
        public void find_files_should_return_all_the_files_from_the_combination_found_in_the_cache()
        {
            ClassUnderTest.FindFiles(theCombination.Name).ShouldHaveTheSameElementsAs(theFiles);
        }
    }

    [TestFixture]
    public class when_finding_files_and_name_only_matches_a_file : InteractionContext<ContentPlanner>
    {
        private AssetFile theFile;

        protected override void beforeEach()
        {
            theFile = new AssetFile("script.js");

            MockFor<IAssetCombinationCache>().Stub(x => x.FindCombination(theFile.Name)).Return(null);

            MockFor<IAssetPipeline>().Stub(x => x.Find(theFile.Name)).Return(theFile);
        }

        [Test]
        public void find_files_should_only_return_the_one_file_found_from_IAssetPipeline()
        {
            ClassUnderTest.FindFiles(theFile.Name).Single().ShouldBeTheSameAs(theFile);
        }
    }
}