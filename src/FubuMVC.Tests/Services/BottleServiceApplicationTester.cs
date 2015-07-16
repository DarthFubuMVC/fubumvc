using System;
using System.Collections.Generic;
using FubuMVC.Core.Services;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Services
{
    [TestFixture]
    public class BottleServiceApplicationTester
    {
        [Test]
        public void concrete_type_of_application_loader_with_default_ctor_is_a_candidate()
        {
            BottleServiceApplication.IsLoaderTypeCandidate(typeof (FakeApplicationLoader))
                .ShouldBeTrue();
        }

        [Test]
        public void application_loader_is_not_candidate_if_abstract()
        {
            BottleServiceApplication.IsLoaderTypeCandidate(typeof(AbstractApplicationLoader))
                .ShouldBeFalse();
        }

        [Test]
        public void application_loader_is_not_candidate_if_no_default_ctor()
        {
            BottleServiceApplication.IsLoaderTypeCandidate(typeof(TemplatedApplicationLoader))
                .ShouldBeFalse();
        }

        [Test]
        public void application_source_with_default_ctor_and_concrete_is_candidate()
        {
            BottleServiceApplication.IsLoaderTypeCandidate(typeof(GoodApplicationSource))
                .ShouldBeTrue();
        }

        [Test]
        public void application_source_with_no_default_ctor_is_not_candidate()
        {
            BottleServiceApplication.IsLoaderTypeCandidate(typeof(AbstractApplicationSource))
                .ShouldBeFalse();
        }

        [Test]
        public void application_source_without_default_ctor_is_not_a_candidate()
        {
            BottleServiceApplication.IsLoaderTypeCandidate(typeof(TemplatedApplicationSource))
                .ShouldBeFalse();
        }

        [Test]
        public void build_application_loader_for_application_loader_type()
        {
            BottleServiceApplication.BuildApplicationLoader(typeof(FakeApplicationLoader))
                .ShouldBeOfType<FakeApplicationLoader>();
        }

        [Test]
        public void building_an_application_loader_for_application_source()
        {
            BottleServiceApplication.BuildApplicationLoader(typeof (GoodApplicationSource))
                .ShouldBeOfType<ApplicationLoader<GoodApplicationSource, Application, IDisposable>>();
        }


        [Test]
        public void building_an_activation_loader_for_a_bad_type_thows()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                BottleServiceApplication.BuildApplicationLoader(GetType());
            });
        }

        [Test]
        public void find_loader_types()
        {
            var types = BottleServiceApplication.FindLoaderTypes();
            types.ShouldContain(typeof(GoodApplicationSource));
            types.ShouldContain(typeof(FakeApplicationLoader));
        }

        [Test]
        public void finds_bootstrapper_by_name()
        {
            BottleServiceApplication.FindLoader(typeof(FakeApplicationLoader).AssemblyQualifiedName)
                .ShouldBeOfType<FakeApplicationLoader>();
        }

        [Test]
        public void finds_bootstrapper_by_name_for_a_source()
        {
            BottleServiceApplication.FindLoader(typeof(GoodApplicationSource).AssemblyQualifiedName)
                .ShouldBeOfType<ApplicationLoader<GoodApplicationSource, Application, IDisposable>>();
        }

        [Test]
        public void blows_up_if_more_than_one_application_source()
        {
            Exception<Exception>.ShouldBeThrownBy(() => {
                BottleServiceApplication.FindLoader(null);
            }).Message.ShouldContain("Found multiple candidates, you may need to specify an explicit selection in the bottle-service.config file.");
        }
    }

    public class TemplatedApplicationLoader : IApplicationLoader
    {
        public TemplatedApplicationLoader(string name)
        {
        }

        public IDisposable Load()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class AbstractApplicationLoader : IApplicationLoader
    {
        public IDisposable Load()
        {
            throw new NotImplementedException();
        }
    }

    public class FakeApplicationLoader : IApplicationLoader
    {
        public IDisposable Load()
        {
            throw new NotImplementedException();
        }
    }

    public class Application : IApplication<IDisposable>
    {
        public IDisposable Bootstrap()
        {
            throw new NotImplementedException();
        }
    }

    public class GoodApplicationSource : IApplicationSource<Application, IDisposable>
    {
        public Application BuildApplication()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class AbstractApplicationSource : IApplicationSource<Application, IDisposable>
    {
        public Application BuildApplication()
        {
            throw new NotImplementedException();
        }
    }

    public class TemplatedApplicationSource : IApplicationSource<Application, IDisposable>
    {
        public TemplatedApplicationSource(string name)
        {
        }

        public Application BuildApplication()
        {
            throw new NotImplementedException();
        }
    }

}