using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Projections
{
    
    public class SimpleValueSource
    {
        [Fact]
        public void retrieve_value_by_accessor()
        {
            var target = new SimpleValues<SimpleClass>(new SimpleClass{
                Age = 37,
                Name = "Jeremy"
            });

            var ageAccessor = ReflectionHelper.GetAccessor<SimpleClass>(x => x.Age);
            var nameAccessor = ReflectionHelper.GetAccessor<SimpleClass>(x => x.Name);

            target.ValueFor(ageAccessor).ShouldBe(37);
            target.ValueFor(nameAccessor).ShouldBe("Jeremy");
        }
    }

    public class SimpleClass
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}