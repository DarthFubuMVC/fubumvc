using System.Linq;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class ContentPlanPreviewerTester
    {
        [Test]
        public void try_it_against_a_3_deep_hierarchy()
        {
            var theFiles = new AssetFile[]{
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

            var combo1 = plan.Combine(new IContentSource[] { read1, read2 });
            var combo2 = plan.Combine(new IContentSource[] { read0, combo1, read3 });


            var previewer = new ContentPlanPreviewer();
            plan.AcceptVisitor(previewer);

            previewer.WriteToDebug();

            previewer.ToFullDescription().ShouldEqual(@"
Combination
  FileRead:a.js
  Combination
    FileRead:b.js
    FileRead:c.js
  FileRead:d.js
".Trim());
        }
    }
}