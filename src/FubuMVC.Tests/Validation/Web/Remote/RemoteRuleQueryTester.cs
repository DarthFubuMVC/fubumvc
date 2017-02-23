using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.Remote;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.Remote
{
    
    public class RemoteRuleQueryTester
    {
        private RequiredFieldRule theRule;
        private IRemoteRuleFilter f1;
        private IRemoteRuleFilter f2;
        private RemoteRuleQuery theQuery;

        public RemoteRuleQueryTester()
        {
            theRule = new RequiredFieldRule();
            f1 = MockRepository.GenerateStub<IRemoteRuleFilter>();
            f2 = MockRepository.GenerateStub<IRemoteRuleFilter>();

            theQuery = new RemoteRuleQuery(new[] { f1, f2 });
        }


        [Fact]
        public void matches_if_any_filters_match()
        {
            f1.Stub(x => x.Matches(theRule)).Return(true);
            f2.Stub(x => x.Matches(theRule)).Return(false);

            theQuery.IsRemote(theRule).ShouldBeTrue();
        }

        [Fact]
        public void does_not_match_no_filters_match()
        {
            f1.Stub(x => x.Matches(theRule)).Return(false);
            f2.Stub(x => x.Matches(theRule)).Return(false);

            theQuery.IsRemote(theRule).ShouldBeFalse();
        }
    }
}