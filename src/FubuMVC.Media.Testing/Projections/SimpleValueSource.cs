using FubuCore.Reflection;
using FubuMVC.Media.Projections;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Media.Testing.Projections
{
    [TestFixture]
    public class SimpleValueSource
    {
        [Test]
        public void retrieve_value_by_accessor()
        {
            var target = new SimpleValues<SimpleClass>(new SimpleClass{
                Age = 37,
                Name = "Jeremy"
            });

            var ageAccessor = ReflectionHelper.GetAccessor<SimpleClass>(x => x.Age);
            var nameAccessor = ReflectionHelper.GetAccessor<SimpleClass>(x => x.Name);

            target.ValueFor(ageAccessor).ShouldEqual(37);
            target.ValueFor(nameAccessor).ShouldEqual("Jeremy");
        }
    }

    public class SimpleClass
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}