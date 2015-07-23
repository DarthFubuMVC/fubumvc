using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Projections;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class pascal_to_camel_casing_names_policy
    {
        [Test]
        public void with_the_camel_casing_correction()
        {
            var projection = new Projection<SomeTarget>();

            projection.CamelCaseAttributeNames();
            projection.Value(x => x.Name);
            projection.Value(x => x.Age);
            projection.Child(x => x.Child).Configure(_ => {
                _.Value(x => x.Name);
            });



            var node = new DictionaryMediaNode();
            var someTarget = new SomeTarget
            {
                Active = true,
                Age = 40,
                Name = "Jeremy",
                Child =  new SomeChild{Name = "Max"}
            };

            projection.As<IProjection<SomeTarget>>().Write(new ProjectionContext<SomeTarget>(new InMemoryServiceLocator(), someTarget), node);

            node.Values["name"].ShouldBe("Jeremy");
            node.Values["age"].ShouldBe(40);

            node.Values["child"].As<IDictionary<string, object>>()["name"].ShouldBe("Max");

        }

        [Test]
        public void with_the_normal_casing()
        {
            var projection = new Projection<SomeTarget>();

            //projection.CamelCaseAttributeNames();
            projection.Value(x => x.Name);
            projection.Value(x => x.Age);
            projection.Child(x => x.Child).Configure(_ =>
            {
                _.Value(x => x.Name);
            });


            var node = new DictionaryMediaNode();
            var someTarget = new SomeTarget
            {
                Active = true,
                Age = 40,
                Name = "Jeremy",
                Child = new SomeChild { Name = "Max" }
            };

            projection.As<IProjection<SomeTarget>>().Write(new ProjectionContext<SomeTarget>(new InMemoryServiceLocator(), someTarget), node);

            node.Values["Name"].ShouldBe("Jeremy");
            node.Values["Age"].ShouldBe(40);

            node.Values["Child"].As<IDictionary<string, object>>()["Name"].ShouldBe("Max");

        }
    }

    public class SomeTarget
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Active { get; set; }

        public SomeChild Child { get; set; }
    }

    public class SomeChild
    {
        public string Name { get; set; }
    }
}