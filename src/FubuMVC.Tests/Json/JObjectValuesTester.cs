using System;
using System.Linq;
using FubuCore.Binding;
using FubuMVC.Core.Json;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Json
{
    [TestFixture]
    public class JObjectValuesTester
    {
        private JObjectValues valuesFor(string json)
        {
            json = json.Replace("\"", "'");


            return new JObjectValues(json);
        }

        [Test]
        public void has()
        {
            var values = valuesFor("{a:1, b:2, c:3}");

            values.Has("a").ShouldBeTrue();
            values.Has("b").ShouldBeTrue();
            values.Has("c").ShouldBeTrue();
            values.Has("d").ShouldBeFalse();
            values.Has("e").ShouldBeFalse();
        }

        [Test]
        public void get()
        {
            var values = valuesFor("{a:'1', b:2, c:3}");

            values.Get("a").ShouldBe("1");
            values.Get("b").ShouldBe("2");
            values.Get("c").ShouldBe("3");
        }

        [Test]
        public void has_child()
        {
            var values = valuesFor("{child1:'1', child2:{a:'1', b:'2'}}");

            values.HasChild("child1").ShouldBeFalse();
            values.HasChild("child3").ShouldBeFalse();

            values.HasChild("child2").ShouldBeTrue();
        }

        [Test]
        public void get_child()
        {
            var values = valuesFor("{child1:'1', child2:{a:'1', b:'2'}}");
            var child = values.GetChild("child2");

            child.Get("a").ShouldBe("1");
            child.Get("b").ShouldBe("2");
        }

        [Test]
        public void get_children_with_no_children_is_an_empty_array()
        {
            var values = valuesFor("{child1:'1', child2:{a:'1', b:'2'}}");
            values.GetChildren("different").Any().ShouldBeFalse();
        }

        [Test]
        public void get_children_with_values()
        {
            var values = valuesFor("{children:[{a:'1'}, {a: '2'}, {a:'3'}]}");

            var children = values.GetChildren("children");

            children.Count().ShouldBe(3);
            children.Select(x => x.Get("a")).ShouldHaveTheSameElementsAs("1", "2", "3");
        }

        [Test]
        public void value_positive()
        {
            var values = valuesFor("{a:1, b:2, c:3}");
            var action = MockRepository.GenerateMock<Action<BindingValue>>();

            values.Value("a", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke(new BindingValue{
                RawKey = "a",
                RawValue = "1",
                Source = values.Provenance
            }));
        }

        [Test]
        public void value_negative()
        {
            var values = valuesFor("{a:1, b:2, c:3}");

            values.Value("nonexistent", v =>
            {
                Assert.Fail("Should not call me");
            }).ShouldBeFalse();
        }
    }
}