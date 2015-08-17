using FubuCore;
using NUnit.Framework;
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
    }

    public class FakeSerenitySystem : SerenitySystem
    {
        public int BeforeAllWasCalled;
        public int BeforeEachWasCalled;

        public IContainer LastScope;

        protected override void beforeAll()
        {
            BeforeAllWasCalled++;
        }

        protected override void afterEach(IContainer scope, ISpecContext context)
        {
            base.afterEach(scope, context);
        }

        protected override void beforeEach(IContainer scope)
        {
            BeforeEachWasCalled++;
            LastScope = scope;
        }

        protected override void afterAll()
        {
            base.afterAll();
        }
    }
}