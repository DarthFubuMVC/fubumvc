using System;
using FubuLocalization;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using FubuCore.Reflection;

namespace FubuMVC.Navigation.Testing
{
    [TestFixture]
    public class NavigationRootPolicyTester
    {
        [Test]
        public void is_after_navigation()
        {
            // This is important.  This has to be like this in order to work 
            // correctly in the configuration ordering
            typeof (NavigationRootPolicy).GetAttribute<ConfigurationTypeAttribute>()
                .Type.ShouldEqual(ConfigurationType.ByNavigation);
        }

        private static BehaviorGraph buildGraph(Action<FubuRegistry> configuration)
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<OneController>();
            registry.Actions.IncludeType<TwoController>();
            registry.Actions.IncludeType<ThreeController>();

            configuration(registry);

            return BehaviorGraph.BuildFrom(registry);
        }

        [Test]
        public void add_roles_by_name()
        {
            var graph = buildGraph(x =>
            {
                x.Import<NavigationRegistryExtension>();

                x.Policies.Add<NavigationRegistry>(r =>
                {
                    r.ForMenu("Menu1");
                    r.Add += MenuNode.ForAction<OneController>("something", c => c.Go());
                    r.Add += MenuNode.ForAction<OneController>("else", c => c.Query(null));
                    r.Add += MenuNode.ForAction<OneController>("different", c => c.Report());
                });

                x.Policies.Add<NavigationRootPolicy>(policy =>
                {
                    policy.ForKey("Menu1");
                    policy.RequireRole("role1");
                    policy.RequireRole("role2");
                });
            });

            // positive cases
            graph.BehaviorFor<OneController>(c => c.Go()).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("role1", "role2");
            graph.BehaviorFor<OneController>(c => c.Query(null)).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("role1", "role2");
            graph.BehaviorFor<OneController>(c => c.Report()).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("role1", "role2");


            graph.BehaviorFor<TwoController>(c => c.Go()).Authorization.AllowedRoles().Any().ShouldBeFalse();
            graph.BehaviorFor<ThreeController>(c => c.Go()).Authorization.AllowedRoles().Any().ShouldBeFalse();
        }

        [Test]
        public void add_roles_by_name_multiple_menus()
        {
            var graph = buildGraph(x =>
            {
                x.Import<NavigationRegistryExtension>();

                x.Policies.Add<NavigationRegistry>(r =>
                {
                    r.ForMenu("Menu1");
                    r.Add += MenuNode.ForAction<OneController>("something", c => c.Go());
                    r.Add += MenuNode.ForAction<OneController>("else", c => c.Query(null));
                    r.Add += MenuNode.ForAction<OneController>("different", c => c.Report());

                    r.ForMenu("Menu2");
                    r.Add += MenuNode.ForAction<TwoController>("something", c => c.Go());
                    r.Add += MenuNode.ForAction<TwoController>("else", c => c.Query(null));
                    r.Add += MenuNode.ForAction<TwoController>("different", c => c.Report());
                });

                x.Policies.Add<NavigationRootPolicy>(policy =>
                {
                    policy.ForKey("Menu1");
                    policy.RequireRole("role1");
                    policy.RequireRole("role2");

                    policy.ForKey("Menu2");
                    policy.RequireRole("role3");
                });
            });

            // positive cases
            graph.BehaviorFor<OneController>(c => c.Go()).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("role1", "role2");
            graph.BehaviorFor<OneController>(c => c.Query(null)).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("role1", "role2");
            graph.BehaviorFor<OneController>(c => c.Report()).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("role1", "role2");


            graph.BehaviorFor<TwoController>(c => c.Go()).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("role3");
            graph.BehaviorFor<ThreeController>(c => c.Go()).Authorization.AllowedRoles().Any().ShouldBeFalse();
        }

        [Test]
        public void add_roles_by_key()
        {
            var key = StringToken.FromKeyString("Menu1");

            var graph = buildGraph(x =>
            {
                x.Import<NavigationRegistryExtension>();

                x.Policies.Add<NavigationRegistry>(r =>
                {
                    r.ForMenu(key);
                    r.Add += MenuNode.ForAction<OneController>("something", c => c.Go());
                    r.Add += MenuNode.ForAction<OneController>("else", c => c.Query(null));
                    r.Add += MenuNode.ForAction<OneController>("different", c => c.Report());
                });

                x.Policies.Add<NavigationRootPolicy>(policy =>
                {
                    policy.ForKey(key);
                    policy.RequireRole("role1");
                    policy.RequireRole("role2");
                });
            });

            // positive cases
            graph.BehaviorFor<OneController>(c => c.Go()).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("role1", "role2");
            graph.BehaviorFor<OneController>(c => c.Query(null)).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("role1", "role2");
            graph.BehaviorFor<OneController>(c => c.Report()).Authorization.AllowedRoles().ShouldHaveTheSameElementsAs("role1", "role2");


            graph.BehaviorFor<TwoController>(c => c.Go()).Authorization.AllowedRoles().Any().ShouldBeFalse();
            graph.BehaviorFor<ThreeController>(c => c.Go()).Authorization.AllowedRoles().Any().ShouldBeFalse();
        }

        [Test]
        public void wrap_with_chrome()
        {
            var graph = buildGraph(x =>
            {
                x.Import<NavigationRegistryExtension>();

                x.Policies.Add<NavigationRegistry>(r =>
                {
                    r.ForMenu("Menu1");
                    r.Add += MenuNode.ForAction<OneController>("something", c => c.Go());
                    r.Add += MenuNode.ForAction<OneController>("else", c => c.Query(null));
                    r.Add += MenuNode.ForAction<OneController>("different", c => c.Report());
                });

                x.Policies.Add<NavigationRootPolicy>(policy =>
                {
                    policy.ForKey("Menu1");

                    policy.WrapWithChrome<FakeChrome>();
                });
            });

            // positive cases
            
            graph.BehaviorFor<OneController>(c => c.Query(null)).OfType<ChromeNode>().Single().ContentType.ShouldEqual(typeof(FakeChrome));
            graph.BehaviorFor<OneController>(c => c.Report()).OfType<ChromeNode>().Single().ContentType.ShouldEqual(typeof(FakeChrome));


            // Go() does not have any output type
            graph.BehaviorFor<OneController>(c => c.Go()).OfType<ChromeNode>().Any().ShouldBeFalse();

            graph.BehaviorFor<TwoController>(c => c.Report()).OfType<ChromeNode>().Any().ShouldBeFalse();
            graph.BehaviorFor<ThreeController>(c => c.Report()).OfType<ChromeNode>().Any().ShouldBeFalse();
        }

        public class FakeChrome : ChromeContent{}

        public class SimpleInputModel
        {
        }

        public class SimpleOutputModel
        {
        }

        public interface IPattern
        {
            void Go();
            SimpleOutputModel Report();
            SimpleOutputModel Query(SimpleInputModel model);
        }

        public class OneController : IPattern
        {
            public void Go()
            {
            }


            public SimpleOutputModel Report()
            {
                return new SimpleOutputModel();
            }

            public SimpleOutputModel Query(SimpleInputModel model)
            {
                return new SimpleOutputModel();
            }

            public static void MethodThatShouldNotBeHere()
            {
            }
        }

        public class TwoController
        {
            public void Go()
            {
            }

            public SimpleOutputModel Report()
            {
                return new SimpleOutputModel();
            }

            public SimpleOutputModel Query(SimpleInputModel model)
            {
                return new SimpleOutputModel();
            }
        }

        public class ThreeController
        {
            public void Go()
            {
            }

            public SimpleOutputModel Report()
            {
                return new SimpleOutputModel();
            }

            public SimpleOutputModel Query(SimpleInputModel model)
            {
                return new SimpleOutputModel();
            }
        }


    }

}