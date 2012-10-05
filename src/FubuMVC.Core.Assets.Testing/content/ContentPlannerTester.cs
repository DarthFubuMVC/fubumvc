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
        public void should_cleanly_return_an_empty_enumerable_for_happy_404_when_findfiles_cannot_find_anything()
        {
            MockFor<IAssetCombinationCache>().Stub(x => x.FindCombination("script1")).Return(null);
            MockFor<IAssetFileGraph>().Stub(x => x.Find("script1")).Return(null);

            ClassUnderTest.FindFiles("script1").Any().ShouldBeFalse();
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
            var read0 = plan.GetAllSources().ElementAt(0);
            var read1 = plan.GetAllSources().ElementAt(1);
            var read2 = plan.GetAllSources().ElementAt(2);
            var read3 = plan.GetAllSources().ElementAt(3);

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
}