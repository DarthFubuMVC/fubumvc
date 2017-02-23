using System;
using System.Security.Principal;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authorization;
using Shouldly;
using Xunit;

namespace FubuMVC.IntegrationTesting.Security.Authorization
{
    
    public class AuthorizationPreviewServiceIntegratedTester : IDisposable
    {
        private FubuRuntime _runtime;



        public void Dispose()
        {
            _runtime?.Dispose();
        }

        private AuthorizationPreviewService withAuthorizationRules(Action<BehaviorGraph> configure)
        {
            var registry = new FubuRegistry();


            registry.Actions.IncludeType<OneController>();
            registry.Actions.IncludeType<TwoController>();

            registry.Configure(configure);


            _runtime = registry.ToRuntime();


            return _runtime.Get<AuthorizationPreviewService>();
        }


        private void userHasRoles(params string[] roles)
        {
            var principal = new GenericPrincipal(new GenericIdentity("somebody"), roles);
            Thread.CurrentPrincipal = principal;
        }

        [Fact]
        public void positive_case_by_model_only()
        {
            userHasRoles("a");
            withAuthorizationRules(
                graph => { graph.ChainFor<OneController>(x => x.M1(null)).Authorization.AddRole("a"); })
                .IsAuthorized(new Model1())
                .ShouldBeTrue();
        }


        [Fact]
        public void positive_case_by_model_only_when_there_are_no_authorization_rules_on_a_chain()
        {
            userHasRoles("a");
            withAuthorizationRules(graph => { }).IsAuthorized(new Model1()).ShouldBeTrue();
        }

        [Fact]
        public void negative_case_by_model_only()
        {
            userHasRoles("a");
            var service =
                withAuthorizationRules(
                    graph => { graph.ChainFor<OneController>(x => x.M1(null)).Authorization.AddRole("not a"); });

            service.IsAuthorized(new Model1()).ShouldBeFalse();
        }

        [Fact]
        public void positive_by_model_only_with_multiple_roles()
        {
            userHasRoles("b", "c");
            withAuthorizationRules(graph =>
            {
                graph.ChainFor<OneController>(x => x.M1(null)).Authorization.AddRole("a");
                graph.ChainFor<OneController>(x => x.M1(null)).Authorization.AddRole("b");
            }).IsAuthorized(new Model1()).ShouldBeTrue();
        }

        [Fact]
        public void positive_by_model_and_category()
        {
            userHasRoles("a");

            var service =
                withAuthorizationRules(
                    graph => { graph.ChainFor<OneController>(x => x.M4(null)).Authorization.AddRole("a"); });

            service.IsAuthorized(new UrlModel(), "different").ShouldBeTrue();
        }


        [Fact]
        public void negative_by_model_and_category()
        {
            userHasRoles("a");

            var service =
                withAuthorizationRules(
                    graph => { graph.ChainFor<OneController>(x => x.M4(null)).Authorization.AddRole("not a"); });

            service.IsAuthorized(new UrlModel(), "different").ShouldBeFalse();
        }

        [Fact]
        public void positive_by_controller_method_expression()
        {
            userHasRoles("a");

            var service =
                withAuthorizationRules(
                    graph => { graph.ChainFor<OneController>(x => x.M2()).Authorization.AddRole("a"); });

            service.IsAuthorized<OneController>(x => x.M2()).ShouldBeTrue();
        }


        [Fact]
        public void negative_by_controller_method_expression()
        {
            userHasRoles("a");

            var service =
                withAuthorizationRules(
                    graph => { graph.ChainFor<OneController>(x => x.M2()).Authorization.AddRole("not a"); });

            service.IsAuthorized<OneController>(x => x.M2()).ShouldBeFalse();
        }

        [Fact]
        public void positive_for_new_entity()
        {
            userHasRoles("a");

            var service =
                withAuthorizationRules(
                    graph => { graph.ChainFor<TwoController>(x => x.M2()).Authorization.AddRole("a"); });

            service.IsAuthorizedForNew(typeof (UrlModel)).ShouldBeTrue();
        }


        [Fact]
        public void negative_for_new_entity()
        {
            userHasRoles("a");

            var service =
                withAuthorizationRules(
                    graph => { graph.ChainFor<TwoController>(x => x.M2()).Authorization.AddRole("not a"); });

            service.IsAuthorizedForNew(typeof (UrlModel)).ShouldBeFalse();
        }

    }


    public class OneController
    {
        [UrlPattern("find/{Name}")]
        public void MethodWithPattern(ModelWithInputs inputs)
        {
        }

        public void A(Model6 input)
        {
        }

        public void B(Model7 input)
        {
        }

        public void M1(Model1 input)
        {
        }

        public Model1 M2()
        {
            return null;
        }

        //public void M3() { }

        public void M5(Model3 input)
        {
        }

        [UrlRegistryCategory("different")]
        public void M4(UrlModel model)
        {
        }
    }

    public class TwoController
    {
        //public void M1() { }

        [UrlForNew(typeof (UrlModel))]
        public Model4 M2()
        {
            throw new NotImplementedException();
        }

        //public void M3() { }
        public void M4(UrlModel model)
        {
        }
    }

    public class ModelWithInputs
    {
        public string Name { get; set; }
    }

    public class Model1
    {
    }

    public class Model2
    {
    }

    public class Model3
    {
    }

    public class Model4
    {
    }

    public class Model5
    {
    }

    public class Model6
    {
    }

    public class Model7
    {
    }

    public class ModelWithNoChain
    {
    }

    public class ModelWithoutNewUrl
    {
    }


    public class UrlModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class SubclassUrlModel : UrlModel
    {
    }
}