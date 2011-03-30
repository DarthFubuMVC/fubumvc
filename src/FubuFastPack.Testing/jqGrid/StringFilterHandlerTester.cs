using FubuCore;
using FubuFastPack.Querying;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class StringFilterHandlerTester
    {
        [Test]
        public void to_filter_rule()
        {
            var handler = new StringFilterHandler(OperatorKeys.CONTAINS, s => s.Contains(null));
            var request = new FilterRequest<Case>(new Criteria(){
                value = "Open"
            }, new ObjectConverter(), c => c.Condition);

            var rule = handler.ToFilterRule(request);

            rule.Accessor.ShouldEqual(request.Accessor);
            rule.Operator.ShouldEqual(OperatorKeys.CONTAINS);
            rule.Value.ShouldEqual("Open");
        }
    }
}