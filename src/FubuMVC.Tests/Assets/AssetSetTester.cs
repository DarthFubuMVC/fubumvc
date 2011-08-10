using System.Linq;
using FubuMVC.Core.Assets;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetSetTester
    {
        private AssetGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            theGraph = new AssetGraph();
        }

        [Test]
        public void find_all_scripts_if_just_referring_to_script_files()
        {
            var theSet = new AssetSet();
            theSet.Add("a");
            theSet.Add("b");
            theSet.Add("c");

            theSet.FindScripts(theGraph);

            theSet.AllFileDependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void find_all_scripts_if_the_set_refers_to_another_set()
        {
            theGraph.AddToSet("1", "A");
            theGraph.AddToSet("1", "B");
            theGraph.AddToSet("1", "C");

            var theSet = new AssetSet();
            theSet.Add("1");

            theGraph.AssetSetFor("1").FindScripts(theGraph);
            theSet.FindScripts(theGraph);

            theSet.AllFileDependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("A", "B", "C");
        }

        [Test]
        public void find_all_scripts_from_a_mix_of_files_and_sets()
        {
            theGraph.AddToSet("1", "A");
            theGraph.AddToSet("1", "B");
            theGraph.AddToSet("1", "C");

            var theSet = new AssetSet();
            theSet.Add("1");
            theSet.Add("D");

            theGraph.AssetSetFor("1").FindScripts(theGraph);
            theSet.FindScripts(theGraph);

            theSet.AllFileDependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("A", "B", "C", "D");
        }

        [Test]
        public void find_all_scripts_2_deep_child_sets_with_files()
        {
            theGraph.AddToSet("1", "A");
            theGraph.AddToSet("1", "B");
            theGraph.AddToSet("1", "2");
            theGraph.AddToSet("2", "C");
            theGraph.AddToSet("2", "D");

            var theSet = new AssetSet();
            theSet.Add("1");
            theSet.Add("E");

            theGraph.AssetSetFor("1").FindScripts(theGraph);
            theGraph.AssetSetFor("2").FindScripts(theGraph);
            theSet.FindScripts(theGraph);

            theSet.AllFileDependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("A", "B", "C", "D", "E");
        }

        [Test]
        public void should_not_return_duplicates()
        {
            theGraph.AddToSet("1", "A");
            theGraph.AddToSet("1", "B");
            theGraph.AddToSet("1", "2");
            theGraph.AddToSet("2", "C");
            theGraph.AddToSet("2", "D");

            var theSet = new AssetSet();
            theSet.Add("1");
            theSet.Add("2");
            theSet.Add("E");

            theGraph.AssetSetFor("1").FindScripts(theGraph);
            theGraph.AssetSetFor("2").FindScripts(theGraph);
            theSet.FindScripts(theGraph);

            theSet.AllFileDependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("A", "B", "C", "D", "E");
        }
    }
}