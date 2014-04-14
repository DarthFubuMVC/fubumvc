using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Projections;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class ProjectMyself_handling_in_an_accessor_projection
    {
        [Test]
        public void should_just_use_IProjectMyself_Project_in_accessor_projection()
        {
            var projection = AccessorProjection<ComplexValueHolder, ComplexValue>.For(x => x.Value);
            var target = new ComplexValueHolder
            {
                Value = new ComplexValue {Name = "Jeremy", Age = 38}
            };

            var context = new ProjectionContext<ComplexValueHolder>(new InMemoryServiceLocator(), target);

            var node = new DictionaryMediaNode();
            projection.As<IProjection<ComplexValueHolder>>().Write(context, node);

            node.Values["Value"].As<IDictionary<string, object>>()["Name"].ShouldEqual("Jeremy");
            node.Values["Value"].As<IDictionary<string, object>>()["Age"].ShouldEqual(38);
        }

        [Test]
        public void should_just_use_IProjectMyself_Project_in_accessor_projection_can_override_the_attribute_name()
        {
            var projection = AccessorProjection<ComplexValueHolder, ComplexValue>.For(x => x.Value);
            projection.Name("different");
            
            var target = new ComplexValueHolder
            {
                Value = new ComplexValue { Name = "Jeremy", Age = 38 }
            };

            var context = new ProjectionContext<ComplexValueHolder>(new InMemoryServiceLocator(), target);

            var node = new DictionaryMediaNode();
            projection.As<IProjection<ComplexValueHolder>>().Write(context, node);

            node.Values["different"].As<IDictionary<string, object>>()["Name"].ShouldEqual("Jeremy");
            node.Values["different"].As<IDictionary<string, object>>()["Age"].ShouldEqual(38);
        }
    }


    public class ComplexValueHolder
    {
        public ComplexValue Value { get; set; }
    }

    public class ComplexValue : IProjectMyself
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public void Project(string attributeName, IMediaNode node)
        {
            var child = node.AddChild(attributeName);
        
            child.SetAttribute("Name", Name);
            child.SetAttribute("Age", Age);
        }
    }
}