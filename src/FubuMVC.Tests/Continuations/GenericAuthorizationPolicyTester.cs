using System;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class GenericAuthorizationPolicyTester : InteractionContext<AuthorizationPolicy<InputModel>>
    {
        private InputModel theModel;
        private AuthorizationRight theRights = AuthorizationRight.Allow;
        private AuthorizationRight theResultingRights;

        protected override void beforeEach()
        {
            theModel = new InputModel();
            MockFor<IAuthorizationRule<InputModel>>().Expect(x => x.RightsFor(theModel)).Return(theRights);

            MockFor<IFubuRequest>().Expect(x => x.Get<InputModel>()).Return(theModel);

            theResultingRights = ClassUnderTest.RightsFor(MockFor<IFubuRequest>());
        }

        [Test]
        public void the_resulting_rights_should_match_the_inner_rule()
        {
            theResultingRights.ShouldEqual(theRights);
        }

        [Test]
        public void should_use_the_model_from_the_fubu_request()
        {
            MockFor<IFubuRequest>().VerifyAllExpectations();
        }

        [Test]
        public void should_exercise_the_inner_rule()
        {
            MockFor<IAuthorizationRule<InputModel>>().VerifyAllExpectations();
        }
    }

    public class InputModel
    {
        
    }
}