using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using FubuTestingSupport;
using KayakTestApplication;
using NUnit.Framework;
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
            FubuApplication.PhysicalRootPath = ".".ToFullPath();

            using (var system = new FubuMvcSystem<TargetApplication>())
            {
                using (var context = system.CreateContext())
                {
                    context.GetService<IRemoteSubsystems>()
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

                system.CreateContext().GetService<IColor>()
                    .ShouldBeOfType<Green>();
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
        public void parallel_path()
        {
            using (var system = new FubuMvcSystem<KayakApplication>("foo"))
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
                    context.GetService<IApplicationUnderTest>().ShouldNotBeNull();
                    system.Recycle();
                    context.GetService<IApplicationUnderTest>().ShouldNotBeNull();
                }

                using (var context = system.CreateContext())
                {
                    context.GetService<IApplicationUnderTest>().ShouldNotBeNull();
                    system.Recycle();
                    context.GetService<IApplicationUnderTest>().ShouldNotBeNull();
                }
            }
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