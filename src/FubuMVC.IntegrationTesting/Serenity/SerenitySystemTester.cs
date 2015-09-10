using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authorization;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity;
using Shouldly;
using StoryTeller;
using StoryTeller.Engine;
using StructureMap;

namespace FubuMVC.IntegrationTesting.Serenity
{
    [TestFixture]
    public class SerenitySystemTester
    {
        [Test]
        public void warmup_will_create_a_fuburuntime()
        {
            using (var system = new SerenitySystem())
            {
                system.As<ISystem>().Warmup().Wait();

                system.Runtime.ShouldNotBeNull();
            }
        }

        [Test]
        public void the_build_runtime_will_be_in_testing_mode()
        {
            using (var system = new SerenitySystem())
            {
                system.As<ISystem>().Warmup().Wait();

                system.Runtime.Mode.InTesting().ShouldBeTrue();
            }
        }

        [Test]
        public void create_a_context_from_warmup_without_failure()
        {
            using (var system = new SerenitySystem())
            {
                system.As<ISystem>().Warmup();

                system.As<ISystem>().CreateContext().ShouldBeOfType<SerenitySystem.SerenityContext>();
            }
        }

        [Test]
        public void create_a_context_without_warmup_without_failure()
        {
            using (var system = new SerenitySystem())
            {
                system.As<ISystem>().CreateContext().ShouldBeOfType<SerenitySystem.SerenityContext>();
            }
        }

        [Test]
        public void beforeAll_is_called_as_part_of_warmup()
        {
            using (var system = new FakeSerenitySystem())
            {
                system.BeforeAllWasCalled.ShouldBe(0);
                system.As<ISystem>().Warmup().Wait();
                system.BeforeAllWasCalled.ShouldBe(1);
            }
        }

        [Test]
        public void before_all_is_called_before_the_first_context_creation_without_warmup()
        {
            using (var system = new FakeSerenitySystem())
            {
                system.BeforeAllWasCalled.ShouldBe(0);
                system.As<ISystem>().CreateContext();
                system.BeforeAllWasCalled.ShouldBe(1);
            }
        }

        [Test]
        public void before_all_is_only_called_before_the_first_context_creation()
        {
            using (var system = new FakeSerenitySystem())
            {
                system.BeforeAllWasCalled.ShouldBe(0);
                system.As<ISystem>().CreateContext();
                system.BeforeAllWasCalled.ShouldBe(1);

                system.As<ISystem>().CreateContext();
                system.BeforeAllWasCalled.ShouldBe(1);

                system.As<ISystem>().CreateContext();
                system.BeforeAllWasCalled.ShouldBe(1);

                system.As<ISystem>().CreateContext();
                system.BeforeAllWasCalled.ShouldBe(1);
            }
        }

        [Test]
        public void before_each_is_called_as_part_of_creating_a_context()
        {
            using (var system = new FakeSerenitySystem())
            {
                system.As<ISystem>().CreateContext();
                system.BeforeEachWasCalled.ShouldBe(1);

                system.As<ISystem>().CreateContext();
                system.BeforeEachWasCalled.ShouldBe(2);
                system.As<ISystem>().CreateContext();
                system.BeforeEachWasCalled.ShouldBe(3);

            }
        }

        [Test]
        public void each_call_to_before_each_gets_a_new_child_container()
        {
            using (var system = new FakeSerenitySystem())
            {
                system.As<ISystem>().CreateContext();
                var scope1 = system.LastScope;

                system.As<ISystem>().CreateContext();
                var scope2 = system.LastScope;
                
                system.As<ISystem>().CreateContext();
                var scope3 = system.LastScope;

                scope1.ShouldNotBeTheSameAs(scope2);
                scope1.ShouldNotBeTheSameAs(scope3);
                scope2.ShouldNotBeTheSameAs(scope3);
            
                scope1.Role.ShouldBe(ContainerRole.ProfileOrChild);
                scope2.Role.ShouldBe(ContainerRole.ProfileOrChild);
                scope3.Role.ShouldBe(ContainerRole.ProfileOrChild);
            }
        }

        [Test]
        public void after_each_is_called_as_context_is_finished()
        {
            using (var system = new FakeSerenitySystem())
            {
                system.AfterEachWasCalled.ShouldBe(0);

                var context1 = system.As<ISystem>().CreateContext();
                var scope1 = system.LastScope;

                var specContext = SpecContext.ForTesting();

                context1.AfterExecution(specContext);

                // uses the same scope for the spec
                system.LastScope.ShouldBeTheSameAs(scope1);
                system.LastContext.ShouldBeSameAs(specContext);
                system.AfterEachWasCalled.ShouldBe(1);
            }
        }

        [Test]
        public void after_all_is_called_as_the_system_is_disposed_if_the_runtime_has_been_created()
        {
            var system = new FakeSerenitySystem();
            system.As<ISystem>().CreateContext();
            system.AfterAllWasCalled.ShouldBe(0);

            system.SafeDispose();

            system.AfterAllWasCalled.ShouldBe(1);
        }

        [Test]
        public void can_retrieve_services_from_the_serenity_context()
        {
            // just some paranoia here
            using (var system = new FakeSerenitySystem())
            {
                system.As<ISystem>().CreateContext().GetService<FubuRuntime>()
                    .ShouldBeTheSameAs(system.Runtime);
            }
        }

        [Test]
        public void security_settings_are_reset_on_each_test_run()
        {
            using (var system = new SerenitySystem())
            {
                system.As<ISystem>().CreateContext();
                system.Runtime.Get<SecuritySettings>().AuthenticationEnabled = false;

                // okay, now create a new context and the settings should go back to everything
                // being enabled
                system.As<ISystem>().CreateContext();
                system.Runtime.Get<SecuritySettings>().AuthenticationEnabled.ShouldBeTrue();

            }
        }

        [Test]
        public void can_create_a_navigation_driver_from_running_serenity_system_with_default_after_navigation()
        {
            using (var system = new FakeSerenitySystem())
            {
                system.As<ISystem>().Warmup().Wait();

                system.Runtime.Get<NavigationDriver>()
                    .AfterNavigation.ShouldBeOfType<NulloAfterNavigation>();

            }
        }

        [Test]
        public void can_create_a_navigation_driver_from_running_serenity_system_with_provided_after_navigation()
        {
            using (var system = new FakeSerenitySystem())
            {
                system.AfterNavigation<FakeAfterNavigation>();
                system.As<ISystem>().Warmup().Wait();

                system.Runtime.Get<NavigationDriver>()
                    .AfterNavigation.ShouldBeOfType<FakeAfterNavigation>();

            }
        }
    }

    public class FakeAfterNavigation : IAfterNavigation
    {
        public void AfterNavigation(IWebDriver driver, string desiredUrl)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class FakeSerenitySystem : SerenitySystem
    {
        public int BeforeAllWasCalled;
        public int AfterAllWasCalled;
        public int BeforeEachWasCalled;
        public int AfterEachWasCalled;

        public IContainer LastScope;
        public ISpecContext LastContext;

        protected override void beforeAll()
        {
            BeforeAllWasCalled++;
        }

        protected override void afterEach(IContainer scope, ISpecContext context)
        {
            AfterEachWasCalled++;
            LastScope = scope;
            LastContext = context;
        }

        protected override void beforeEach(IContainer scope)
        {
            BeforeEachWasCalled++;
            LastScope = scope;
        }

        protected override void afterAll()
        {
            AfterAllWasCalled++;
        }
    }
}