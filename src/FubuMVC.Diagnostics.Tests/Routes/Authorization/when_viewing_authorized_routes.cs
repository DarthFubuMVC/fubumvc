using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Diagnostics.Core.Infrastructure;
using FubuMVC.Diagnostics.Features.Routes.Authorization;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Routes.Authorization
{
    [TestFixture]
    public class when_viewing_authorized_routes : InteractionContext<Features.Routes.Authorization.GetHandler>
    {
        private BehaviorGraph _graph;

        protected override void beforeEach()
        {
            _graph = new FubuRegistry(x =>
                                          {
                                              x.IncludeDiagnostics(true);
                                              x.Applies.ToThisAssembly();
                                              x.Actions.IncludeClassesSuffixedWithController();
                                          }).BuildGraph();

            Container.Inject(_graph);
        }

        [Test]
        public void should_describe_chains_for_each_authorization_rule()
        {
            _graph
                .Behaviors
                .Each(chain =>
                          {
                              if(!chain.Authorization.HasRules())
                              {
                                  MockFor<IAuthorizationDescriptor>()
                                      .Expect(d => d.AuthorizorFor(chain))
                                      .Return(new NulloEndPointAuthorizer());
                                  return;
                              }

                              MockFor<IAuthorizationDescriptor>()
                                      .Expect(d => d.AuthorizorFor(chain))
                                      .Return(MockFor<IEndPointAuthorizor>());
                          });
            

            MockFor<IEndPointAuthorizor>()
                .Expect(a => a.RulesDescriptions())
                .Return(new List<string> {"Test"});

            var model = ClassUnderTest.Execute(new AuthorizationRequestModel());
            var rule = model.Rules.First();

            rule.Name.ShouldEqual("Test");
            rule.Routes.ShouldHaveCount(1);
        }

        public class NulloEndPointAuthorizer : IEndPointAuthorizor
        {
            public AuthorizationRight IsAuthorized(IFubuRequest request)
            {
                return AuthorizationRight.None;
            }

            public IEnumerable<string> RulesDescriptions()
            {
                yield break;
            }
        }

        public class SampleAuthorizedController
        {
            [AllowRole("Test")]
            public SampleViewModel Index(SampleInputModel model)
            {
                return new SampleViewModel();
            }
        }

        public class SampleViewModel { }

        public class SampleInputModel { }
    }
}