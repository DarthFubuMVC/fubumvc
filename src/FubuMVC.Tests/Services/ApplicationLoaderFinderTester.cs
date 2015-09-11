using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Services;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Services
{
    [TestFixture]
    public class ApplicationLoaderFinderTester
    {
        [Test]
        public void concrete_type_of_application_loader_with_default_ctor_is_a_candidate()
        {
            ApplicationLoaderFinder.IsLoaderTypeCandidate(typeof (FakeApplicationLoader))
                .ShouldBeTrue();
        }

        [Test]
        public void application_loader_is_not_candidate_if_abstract()
        {
            ApplicationLoaderFinder.IsLoaderTypeCandidate(typeof (AbstractApplicationLoader))
                .ShouldBeFalse();
        }

        [Test]
        public void application_loader_is_not_candidate_if_no_default_ctor()
        {
            ApplicationLoaderFinder.IsLoaderTypeCandidate(typeof (TemplatedApplicationLoader))
                .ShouldBeFalse();
        }

        [Test]
        public void application_source_with_default_ctor_and_concrete_is_candidate()
        {
            ApplicationLoaderFinder.IsLoaderTypeCandidate(typeof (GoodApplicationSource))
                .ShouldBeTrue();
        }

        [Test]
        public void application_source_with_no_default_ctor_is_not_candidate()
        {
            ApplicationLoaderFinder.IsLoaderTypeCandidate(typeof (AbstractApplicationSource))
                .ShouldBeFalse();
        }

        [Test]
        public void application_source_without_default_ctor_is_not_a_candidate()
        {
            ApplicationLoaderFinder.IsLoaderTypeCandidate(typeof (TemplatedApplicationSource))
                .ShouldBeFalse();
        }

        [Test]
        public void build_application_loader_for_application_loader_type()
        {
            ApplicationLoaderFinder.BuildApplicationLoader(typeof (FakeApplicationLoader))
                .ShouldBeOfType<FakeApplicationLoader>();
        }

        [Test]
        public void building_an_activation_loader_for_a_bad_type_thows()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                () => { ApplicationLoaderFinder.BuildApplicationLoader(GetType()); });
        }

        [Test]
        public void find_loader_types()
        {
            var types = ApplicationLoaderFinder.FindLoaderTypes();
            types.ShouldContain(typeof (GoodApplicationSource));
            types.ShouldContain(typeof (FakeApplicationLoader));
        }

        [Test]
        public void finds_bootstrapper_by_name()
        {
            ApplicationLoaderFinder.FindLoader(typeof (FakeApplicationLoader).AssemblyQualifiedName)
                .ShouldBeOfType<FakeApplicationLoader>();
        }

        [Test]
        public void blows_up_if_more_than_one_application_source()
        {
            Exception<Exception>.ShouldBeThrownBy(() => { ApplicationLoaderFinder.FindLoader(null); })
                .Message.ShouldContain(
                    "Found multiple candidates, you may need to specify an explicit selection in the bottle-service.config file.");
        }

        [Test]
        public void determine_loader_for_fuburegistry()
        {
            ApplicationLoaderFinder.DetermineLoaderType(typeof(StandinFubuRegistry))
                .ShouldBe(typeof(FubuRegistryLoader<StandinFubuRegistry>));
        }

        [Test]
        public void build_loader_for_fuburegistry()
        {
            ApplicationLoaderFinder.BuildApplicationLoader(typeof (StandinFubuRegistry))
                .ShouldBeOfType<FubuRegistryLoader<StandinFubuRegistry>>();
        }
    }

    public class StandinFubuRegistry : FubuRegistry { }

    public class TemplatedApplicationLoader : IApplicationLoader
    {
        public TemplatedApplicationLoader(string name)
        {
        }

        public IDisposable Load(Dictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class AbstractApplicationLoader : IApplicationLoader
    {
        public IDisposable Load(Dictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeApplicationLoader : IApplicationLoader
    {
        public IDisposable Load(Dictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }
    }

    public class Application : FubuRegistry
    {
        public IDisposable Bootstrap()
        {
            throw new NotImplementedException();
        }
    }

    public class GoodApplicationSource : FubuRegistry
    {
        public Application BuildApplication(string directory)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class AbstractApplicationSource : FubuRegistry
    {
        public Application BuildApplication(string directory)
        {
            throw new NotImplementedException();
        }
    }

    public class TemplatedApplicationSource : FubuRegistry
    {
        public TemplatedApplicationSource(string name)
        {
        }

        public Application BuildApplication(string directory)
        {
            throw new NotImplementedException();
        }
    }
}