using FubuMVC.Spark.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{
    [TestFixture]
    public class ViewModelTypeFinderTester 
    {
        private ViewModelTypeFinder ClassUnderTest;

        [SetUp]
        public void beforeEach()
        {
            ClassUnderTest = new ViewModelTypeFinder(new[] { typeof(Bar).Assembly});
        }

        [Test]
        public void should_parse_a_single_generic_argument()
        {
            const string typeName = "FubuMVC.Spark.Tests.SparkModel.Binding.Generic<FubuMVC.Spark.Tests.SparkModel.Binding.Baz>";

            var type = ClassUnderTest.Parse(typeName);

            type.ShouldEqual(typeof(Generic<Baz>));
        }

        [Test]
        public void should_parse_a_mutiple_generic_arguments()
        {
            const string typeName = "FubuMVC.Spark.Tests.SparkModel.Binding.Generic<FubuMVC.Spark.Tests.SparkModel.Binding.Baz, FubuMVC.Spark.Tests.SparkModel.Binding.Bar>";

            var type = ClassUnderTest.Parse(typeName);

            type.ShouldEqual(typeof(Generic<Baz, Bar>));
        }

        [Test]
        public void should_detect_if_type_name_is_generic()
        {
            ViewModelTypeFinder.IsGeneric("System.String").ShouldBeFalse();
            ViewModelTypeFinder.IsGeneric("System.Collections.List<System.String>").ShouldBeTrue();
        }
    }
}