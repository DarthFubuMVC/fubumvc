using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class ChildProjectionTester
    {
        private Projection<Parent> theProjection;
        private Parent theParent;
        private Lazy<IDictionary<string, object>> _dictionary;

        [SetUp]
        public void SetUp()
        {
            theProjection = new Projection<Parent>(DisplayFormatting.RawValues);
            theParent = new Parent();

            _dictionary = new Lazy<IDictionary<string, object>>(() =>
            {
                var runner = new ProjectionRunner(new InMemoryServiceLocator());
                var node = new DictionaryMediaNode();

                runner.Run(theProjection,theParent, node);

                return node.Values;
            });
        }

        [Test]
        public void accessors()
        {
            var projection = new ChildProjection<Parent, Child>(x => x.Child, DisplayFormatting.RawValues);
            projection.As<IProjection<Parent>>().Accessors().Single().ShouldEqual(ReflectionHelper.GetAccessor<Parent>(x => x.Child));
        }

        private IDictionary<string, object> theDictionary
        {
            get
            {
                return _dictionary.Value;
            }
        }

        [Test]
        public void explicit_writing()
        {
            theParent.Child = new Child
            {
                Age = 38,
                Name = "Jeremy"
            };

            theProjection.Child(x => x.Child).WriteWith((c, n) =>
            {
                n.SetAttribute("age", c.Values.ValueFor(x => x.Age));
                n.SetAttribute("name", c.Values.ValueFor(x => x.Name));
            });

            var child = theDictionary.Child("Child");
            child["age"].ShouldEqual(38);
            child["name"].ShouldEqual("Jeremy");
        }

        [Test]
        public void child_projection_when_the_child_is_null()
        {
            theParent.Child = null;
            theProjection.Child(x => x.Child).Include<TheChildProjection>();

            theDictionary.ContainsKey("Child").ShouldBeFalse();
        }

        [Test]
        public void child_projection_when_the_child_is_not_null()
        {
            theParent.Child = new Child{
                Age = 38,
                Name = "Jeremy"
            };

            theProjection.Child(x => x.Child).Include<TheChildProjection>();

            theDictionary.ContainsKey("Child").ShouldBeTrue();

            theDictionary.Child("Child")["Age"].ShouldEqual(38);
            theDictionary.Child("Child")["Name"].ShouldEqual("Jeremy");
        }

        [Test]
        public void override_the_name_of_the_child()
        {
            theParent.Child = new Child
            {
                Age = 38,
                Name = "Jeremy"
            };

            theProjection.Child(x => x.Child).Name("Different").Include<TheChildProjection>();

            theDictionary.ContainsKey("Different").ShouldBeTrue();
            theDictionary.ContainsKey("Child").ShouldBeFalse();

            theDictionary.Child("Different")["Age"].ShouldEqual(38);
            theDictionary.Child("Different")["Name"].ShouldEqual("Jeremy");
        }

        [Test]
        public void define_the_child_projection_inline()
        {
            theProjection.Child(x => x.Child).Configure(x =>
            {
                x.Value(o => o.Name);
            });

            theParent.Child = new Child
            {
                Age = 38,
                Name = "Jeremy"
            };

            theDictionary.ContainsKey("Child").ShouldBeTrue();

            theDictionary.Child("Child").ContainsKey("Age").ShouldBeFalse();
            theDictionary.Child("Child")["Name"].ShouldEqual("Jeremy");
        }


        public class TheChildProjection : Projection<Child>
        {
            public TheChildProjection()
                : base(DisplayFormatting.RawValues)
            {
                Value(x => x.Name);
                Value(x => x.Age);
            }
        }

        public class Parent
        {
            public Child Child { get; set; }
        }

        public class Child
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}