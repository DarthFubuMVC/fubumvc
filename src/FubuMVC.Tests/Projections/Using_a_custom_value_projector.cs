using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Projections;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class Using_a_custom_value_projector
    {
        [Test]
        public void should_just_use_IValueProjector_when_overriden_in_accessor_projection()
        {
            var projection = AccessorProjection<CustomValueHolder, CustomValue>.For(x => x.Value);
            projection.ProjectWith<CustomValueProjector>();

            var target = new CustomValueHolder
            {
                Value = new CustomValue { Name = "Jeremy", Age = 38 }
            };

            var context = new ProjectionContext<CustomValueHolder>(new InMemoryServiceLocator(), target);

            var node = new DictionaryMediaNode();
            projection.As<IProjection<CustomValueHolder>>().Write(context, node);

            node.Values["Value"].As<IDictionary<string, object>>()["Name"].ShouldEqual("Jeremy");
            node.Values["Value"].As<IDictionary<string, object>>()["Age"].ShouldEqual(38);
        }

        [Test]
        public void should_just_use_IValueProjector_when_overriden_in_accessor_projection_and_can_override_the_attribute_name()
        {
            var projection = AccessorProjection<CustomValueHolder, CustomValue>.For(x => x.Value);
            projection.ProjectWith<CustomValueProjector>();
            projection.Name("different");

            var target = new CustomValueHolder
            {
                Value = new CustomValue { Name = "Jeremy", Age = 38 }
            };

            var context = new ProjectionContext<CustomValueHolder>(new InMemoryServiceLocator(), target);

            var node = new DictionaryMediaNode();
            projection.As<IProjection<CustomValueHolder>>().Write(context, node);

            node.Values["different"].As<IDictionary<string, object>>()["Name"].ShouldEqual("Jeremy");
            node.Values["different"].As<IDictionary<string, object>>()["Age"].ShouldEqual(38);
        }
    }


    public class CustomValueProjector : IValueProjector<CustomValue>
    {
        public void Project(string attributeName, CustomValue value, IMediaNode node)
        {
            var child = node.AddChild(attributeName);

            child.SetAttribute("Name", value.Name);
            child.SetAttribute("Age", value.Age);
        }
    }

    public class CustomValueHolder
    {
        public CustomValue Value { get; set; }
    }

    public class CustomValue
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

}
