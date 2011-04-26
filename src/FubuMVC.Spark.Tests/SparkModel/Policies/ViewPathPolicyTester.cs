using System.Collections.Generic;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Binders
{
    [TestFixture]
    public class ViewPathPolicyTester : InteractionContext<ViewPathPolicy>
    {
        [Test]
        public void when_origin_is_host_prefix_is_emtpy()
        {
            var item = new SparkItem("", "", Constants.HostOrigin);
            ClassUnderTest.Apply(item);
            item.ViewPath.ShouldBeEmpty();
        }

        [Test]
        public void when_origin_is_not_host_prefix_is_not_emtpy()
        {
            var item = new SparkItem("", "", "Foo");
            ClassUnderTest.Apply(item);
            item.ViewPath.ShouldNotBeEmpty();
        }

        [Test]
        public void the_items_of_the_same_origin_have_the_same_prefix()
        {
            var baz1 = new SparkItem("", "", "Baz");
            var baz2 = new SparkItem("", "", "Baz");
            var bar1 = new SparkItem("", "", "Bar");
            var bar2 = new SparkItem("", "", "Bar");

            new[] { baz1, baz2, bar1, bar2 }.Each(x => ClassUnderTest.Apply(x));

            baz1.ViewPath.ShouldEqual(baz2.ViewPath);
            bar1.ViewPath.ShouldEqual(bar2.ViewPath);
            baz1.ViewPath.ShouldNotEqual(bar1.ViewPath);
        }
    }
}