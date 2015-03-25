using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags;
using KayakTestApplication;
using NUnit.Framework;
using StoryTeller.Engine;
using StructureMap;

namespace Serenity.Testing
{
    [TestFixture]
    public class FubuMvcSystemTester
    {
        [Test]
        public void register_a_remote_subsystem()
        {
            var system = new FubuMvcSystem(null, () => null);
            system.AddRemoteSubSystem("foo", x => { });

            system.RemoteSubSystemFor("foo").ShouldNotBeNull();
            system.SubSystems.OfType<RemoteSubSystem>().Single().ShouldBeOfType<RemoteSubSystem>();
        }

        [Test]
        public void registers_the_IRemoveSubsystems_with_the_container()
        {
            FubuMvcPackageFacility.PhysicalRootPath = ".".ToFullPath();

            using (var system = new FubuMvcSystem<TargetApplication>())
            {
                using (var context = system.CreateContext())
                {
                    context.Services.GetInstance<IRemoteSubsystems>()
                        .ShouldBeTheSameAs(system);
                }
            }
        }


        [Test]
        public void modify_the_underlying_container()
        {
            using (var system = new FubuMvcSystem<TargetApplication>())
            {
                system.ModifyContainer(x => x.For<IColor>().Use<Green>());

                system.CreateContext().Services.GetInstance<IColor>()
                    .ShouldBeOfType<Green>();
            }
        }

        [Test]
        public void works_with_the_contextual_providers()
        {
            using (var system = new FubuMvcSystem<TargetApplication>())
            {
                system.ModifyContainer(x => {
                    x.For<IContextualInfoProvider>().Add(new FakeContextualProvider("red", "green"));
                    x.For<IContextualInfoProvider>().Add(new FakeContextualProvider("blue", "orange"));
                });

                system.CreateContext().As<IResultsExtension>()
                    .Tags().Select(x => x.Text())
                    .ShouldHaveTheSameElementsAs("red", "green", "blue", "orange");
            }
        }

        [Test]
        public void uses_explicit_physical_path_if_if_exists()
        {
            using (var system = new FubuMvcSystem<TargetApplication>(physicalPath: "c:\\foo"))
            {
                system.Settings.PhysicalPath.ShouldEqual("c:\\foo");
            }
        }

        [Test]
        public void use_default_physical_path_if_none_given()
        {
            using (var system = new FubuMvcSystem<KayakApplication>())
            {
                // assembly name
                system.Settings.PhysicalPath.ShouldEndWith("KayakTestApplication");
            }
        }

        [Test]
        public void parallel_path()
        {
            using (var system = new FubuMvcSystem<KayakApplication>(parallelDirectory: "foo"))
            {
                // assembly name
                system.Settings.PhysicalPath.ShouldEndWith("foo");
            }
        }

        [Test]
        public void can_recycle()
        {
            using (var system = new FubuMvcSystem<TargetApplication>())
            {
                using (var context = system.CreateContext())
                {
                    context.Services.GetInstance<IApplicationUnderTest>().ShouldNotBeNull();
                    system.Recycle();
                    context.Services.GetInstance<IApplicationUnderTest>().ShouldNotBeNull();
                }

                using (var context = system.CreateContext())
                {
                    context.Services.GetInstance<IApplicationUnderTest>().ShouldNotBeNull();
                    system.Recycle();
                    context.Services.GetInstance<IApplicationUnderTest>().ShouldNotBeNull();
                }
            }
        }
    }

    public class FakeContextualProvider : IContextualInfoProvider
    {
        private readonly string[] _colors;

        public FakeContextualProvider(params string[] colors)
        {
            _colors = colors;
        }

        public void Reset()
        {
        }

        public IEnumerable<HtmlTag> GenerateReports()
        {
            return _colors.Select(x => new HtmlTag("span").Text(x));
        }
    }

    public class TargetApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            var container = new Container(x => { x.For<IColor>().Use<Red>(); });

            return FubuApplication.DefaultPolicies().StructureMap(container);
        }
    }

    public interface IColor
    {
    }

    public class Red : IColor
    {
    }

    public class Green : IColor
    {
    }
}