using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class AllowRoleAttributeTester
    {
        private BehaviorGraph graph;

        [TestFixtureSetUp]
        public void SetUp()
        {
            graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions
                .IncludeType<AllowRoleController>()
                .IncludeType<AllowRoleController2>();
            });
        }

        private IEnumerable<string> rolesFor<T>(Expression<Action<T>> method)
        {
            var chain = graph.BehaviorFor(method);
            return chain.Authorization.AllowedRoles();
        }

        private void shouldBeNoRolesFor<T>(Expression<Action<T>> method)
        {
            graph.BehaviorFor(method).Top.Any(x => x is AuthorizationNode).ShouldBeFalse();
        }

        [Test]
        public void should_be_no_roles_on_method_actions_with_no_attribute_when_the_controller_has_not_attributes()
        {
            shouldBeNoRolesFor<AllowRoleController>(x => x.M1());
            shouldBeNoRolesFor<AllowRoleController>(x => x.M4());
            shouldBeNoRolesFor<AllowRoleController>(x => x.M5());
        }

        [Test]
        public void handle_one_role_for_a_method()
        {
            rolesFor<AllowRoleController>(x => x.M3()).ShouldHaveTheSameElementsAs("R3");
        }

        [Test]
        public void handle_multiple_roles_for_a_method()
        {
            rolesFor<AllowRoleController>(x => x.M2()).ShouldHaveTheSameElementsAs("R1", "R2");
        }

        [Test]
        public void roles_with_an_attribute_on_the_handler_class_itself()
        {
            rolesFor<AllowRoleController2>(x => x.M1()).ShouldHaveTheSameElementsAs("R1", "R2");
            rolesFor<AllowRoleController2>(x => x.M2()).ShouldHaveTheSameElementsAs("R1", "R2");
            rolesFor<AllowRoleController2>(x => x.M3()).ShouldHaveTheSameElementsAs("R1", "R2");
            rolesFor<AllowRoleController2>(x => x.M4()).ShouldHaveTheSameElementsAs("R1", "R2");
            rolesFor<AllowRoleController2>(x => x.M5()).ShouldHaveTheSameElementsAs("R1", "R2");
        }
    }

    public class AllowRoleController
    {

        public void M1(){}
        [AllowRole("R1", "R2")]
        public void M2(){}

        [AllowRole("R3")]
        public void M3(){}
        public void M4(){}
        public void M5(){}
    }

    [AllowRole("R1", "R2")]
    public class AllowRoleController2
    {

        public void M1() { }
        public void M2() { }
        public void M3() { }
        public void M4() { }
        public void M5() { }
    }
}