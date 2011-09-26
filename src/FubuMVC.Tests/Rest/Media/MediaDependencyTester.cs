using System;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Rest.Media;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Rest.Media
{
    [TestFixture]
    public class MediaDependencyTester
    {
        private MediaDependency theDependency;

        [SetUp]
        public void SetUp()
        {
            theDependency = new MediaDependency(typeof (IDoer<>), typeof (Thing1));
        }

        [Test]
        public void set_type_with_invalid_type_throws()
        {
            Exception<ArgumentException>.ShouldBeThrownBy(() =>
            {
                theDependency.SetType(typeof(Thing2Doer));
            });
        }

        [Test]
        public void set_value_with_unpluggable_value()
        {
            Exception<ArgumentException>.ShouldBeThrownBy(() =>
            {
                theDependency.SetValue(new Thing2Doer());
            });
        }

        [Test]
        public void set_value_builds_a_value_dependency()
        {
            var doer = new Thing1Doer();
            theDependency.SetValue(doer);

            theDependency.Dependency.ShouldBeOfType<ValueDependency>()
                .Value.ShouldBeTheSameAs(doer);
        }

        [Test]
        public void set_type_builds_a_configured_dependency()
        {
            theDependency.SetType(typeof(Thing1Doer));
            theDependency.Dependency.ShouldBeOfType<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(typeof (Thing1Doer));
        }
    }

    public interface IDoer<T>
    {
        
    }

    public class Doer<T> : IDoer<T>
    {
        
    }

    public class Thing1{}
    public class Thing2{}

    public class Thing1Doer : Doer<Thing1>{}
    public class Thing2Doer : Doer<Thing2>{}
}