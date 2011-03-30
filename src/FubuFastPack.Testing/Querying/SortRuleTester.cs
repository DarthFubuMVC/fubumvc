using FubuFastPack.Querying;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.Querying
{
    [TestFixture]
    public class SortRuleTester
    {
        [Test]
        public void do_not_override_sorting_if_it_exists()
        {
            var rule = SortRule<Case>.Ascending(x => x.Condition);
            var request = new GridDataRequest(){
                SortColumn = "different",
                SortAscending = false
            };

            rule.ApplyDefaultSorting(request);

            request.SortColumn.ShouldEqual("different");
            request.SortAscending.ShouldBeFalse();

        }

        [Test]
        public void do_override_sorting_if_no_sorting_exists()
        {
            var rule = SortRule<Case>.Ascending(x => x.Condition);
            var request = new GridDataRequest()
            {
                SortColumn = null,
                SortAscending = false
            };

            rule.ApplyDefaultSorting(request);

            request.SortColumn.ShouldEqual("Condition");
            request.SortAscending.ShouldBeTrue();
        }
    }
}