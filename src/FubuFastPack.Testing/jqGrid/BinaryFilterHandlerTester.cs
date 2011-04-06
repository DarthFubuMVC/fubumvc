using FubuCore;
using FubuCore.Reflection.Expressions;
using FubuFastPack.Querying;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class BinaryFilterHandlerTester
    {
        [Test]
        public void to_filter_rule()
        {
            var handler = new BinaryFilterHandler<LessThanPropertyOperation>(OperatorKeys.LESSTHAN, t => true);
            var request = new FilterRequest<Case>(new Criteria(){
                value = "5"
            }, new ObjectConverter(), c => c.Integer);

            var rule = handler.ToFilterRule(request);

            rule.Accessor.ShouldEqual(request.Accessor);
            rule.Operator.ShouldEqual(OperatorKeys.LESSTHAN);
            rule.Value.ShouldEqual(5);
        }
    }
}