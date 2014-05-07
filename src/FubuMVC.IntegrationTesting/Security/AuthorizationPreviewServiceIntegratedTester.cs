using System;
using System.Security.Principal;
using System.Threading;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.Security
{
    [TestFixture]
    public class AuthorizationPreviewServiceIntegratedTester
    {
        private AuthorizationPreviewService withAuthorizationRules(Action<BehaviorGraph> configure)
        {
            var registry = new FubuRegistry();
            

            registry.Actions.IncludeType<OneController>();
            registry.Actions.IncludeType<TwoController>();

            registry.Configure(x =>
            {
                x.TypeResolver.AddStrategy<UrlModelForwarder>();
            });

            registry.Configure(configure);

            var container = new Container();
            FubuApplication.For(() => registry).StructureMap(container).Bootstrap();

            return container.GetInstance<AuthorizationPreviewService>();
        }


        private void userHasRoles(params string[] roles)
        {
            var principal = new GenericPrincipal(new GenericIdentity("somebody"), roles);
            Thread.CurrentPrincipal = principal;
        }

        [Test]
        public void positive_case_by_model_only()
        {
            userHasRoles("a");
            withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<OneController>(x => x.M1(null)).Authorization.AddRole("a");
            }).IsAuthorized(new Model1()).ShouldBeTrue();
        }


        [Test]
        public void positive_case_by_model_only_when_there_are_no_authorization_rules_on_a_chain()
        {
            userHasRoles("a");
            withAuthorizationRules(graph =>
            {
            }).IsAuthorized(new Model1()).ShouldBeTrue();
        }

        [Test]
        public void negative_case_by_model_only()
        {
            userHasRoles("a");
            var service = withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<OneController>(x => x.M1(null)).Authorization.AddRole("not a");
            });

            service.IsAuthorized(new Model1()).ShouldBeFalse();
        }

        [Test]
        public void positive_by_model_only_with_multiple_roles()
        {
            userHasRoles("b", "c");
            withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<OneController>(x => x.M1(null)).Authorization.AddRole("a");
                graph.BehaviorFor<OneController>(x => x.M1(null)).Authorization.AddRole("b");
            }).IsAuthorized(new Model1()).ShouldBeTrue();
        }

        [Test]
        public void positive_by_model_and_category()
        {
            userHasRoles("a");

            var service = withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<OneController>(x => x.M4(null)).Authorization.AddRole("a");
            });

            service.IsAuthorized(new UrlModel(), "different").ShouldBeTrue();
        }


        [Test]
        public void negative_by_model_and_category()
        {
            userHasRoles("a");

            var service = withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<OneController>(x => x.M4(null)).Authorization.AddRole("not a");
            });

            service.IsAuthorized(new UrlModel(), "different").ShouldBeFalse();
        }

        [Test]
        public void positive_by_controller_method_expression()
        {
            userHasRoles("a");

            var service = withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<OneController>(x => x.M2()).Authorization.AddRole("a");
            });

            service.IsAuthorized<OneController>(x => x.M2()).ShouldBeTrue();
        }


        [Test]
        public void negative_by_controller_method_expression()
        {
            userHasRoles("a");

            var service = withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<OneController>(x => x.M2()).Authorization.AddRole("not a");
            });

            service.IsAuthorized<OneController>(x => x.M2()).ShouldBeFalse();
        }

        [Test]
        public void positive_for_new_entity()
        {
            userHasRoles("a");

            var service = withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<TwoController>(x => x.M2()).Authorization.AddRole("a");
            });

            service.IsAuthorizedForNew(typeof(UrlModel)).ShouldBeTrue();
        }


        [Test]
        public void negative_for_new_entity()
        {
            userHasRoles("a");

            var service = withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<TwoController>(x => x.M2()).Authorization.AddRole("not a");
            });

            service.IsAuthorizedForNew(typeof(UrlModel)).ShouldBeFalse();
        }

        [Test]
        public void handles_forwarding_correctly_positive_case()
        {
            userHasRoles("a");

            var service = withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<TwoController>(x => x.M4(null)).Authorization.AddRole("a");
            });

            service.IsAuthorized(new SubclassUrlModel()).ShouldBeTrue();
        }


        [Test]
        public void handles_forwarding_correctly_negative_case()
        {
            userHasRoles("a");

            var service = withAuthorizationRules(graph =>
            {
                graph.BehaviorFor<TwoController>(x => x.M4(null)).Authorization.AddRole("not a");
            });

            service.IsAuthorized(new SubclassUrlModel()).ShouldBeFalse();
        }




    }




    public class OneController
    {
        [UrlPattern("find/{Name}")]
        public void MethodWithPattern(ModelWithInputs inputs)
        {

        }

        public void A(Model6 input) { }
        public void B(Model7 input) { }

        public void M1(Model1 input) { }
        public Model1 M2() {
            return null; }
        //public void M3() { }

        public void M5(Model3 input)
        {
        }

        [UrlRegistryCategory("different")]
        public void M4(UrlModel model) { }
    }

    public class TwoController
    {
        //public void M1() { }

        [UrlForNew(typeof(UrlModel))]
        public Model4 M2() { throw new NotImplementedException(); }
        //public void M3() { }
        public void M4(UrlModel model) { }
    }

    public class ModelWithInputs
    {
        public string Name { get; set; }
    }
    public class Model1 { }
    public class Model2 { }
    public class Model3 { }
    public class Model4 { }
    public class Model5 { }
    public class Model6 { }
    public class Model7 { }
    public class ModelWithNoChain { }
    public class ModelWithoutNewUrl { }

    public class UrlModelForwarder : ITypeResolverStrategy
    {
        public bool Matches(object model)
        {
            return model is SubclassUrlModel;
        }

        public Type ResolveType(object model)
        {
            return typeof(UrlModel);
        }
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