using System.Diagnostics;
using FubuMVC.Core.Assets.Combination;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using System.Collections.Generic;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetGrouperTester
    {
        [Test]
        public void simple_group()
        {
            var subjects = new int[]{1, 2, 3, -4, -5, 6, 7};
            var grouper = new AssetGrouper<int>();
            grouper.GroupSubjects(subjects, i => i < 0).Single().ShouldHaveTheSameElementsAs(-4, -5);
        }

        [Test]
        public void multiple_groups()
        {
            var subjects = new int[] { 1, 2, 3, -4, -5, 6, 7, -8, -9, 10, 11, -12 };

            var groups = new AssetGrouper<int>().GroupSubjects(subjects, i => i < 0);
            groups.Count().ShouldEqual(3);

            groups.ElementAt(0).Each(x => Debug.WriteLine(x));

            groups.ElementAt(0).ShouldHaveTheSameElementsAs(-4, -5);
            groups.ElementAt(1).ShouldHaveTheSameElementsAs(-8, -9);
            groups.ElementAt(2).ShouldHaveTheSameElementsAs(-12);
        }
    }
}