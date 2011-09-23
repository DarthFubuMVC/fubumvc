using System;
using FubuMVC.Spark.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{
    [TestFixture]
    public class GenericParserTester
    {
        private GenericParser ClassUnderTest;

        [SetUp]
        public void beforeEach()
        {
            ClassUnderTest = new GenericParser(new[] { typeof(Bar).Assembly, typeof(String).Assembly });
        }

        [Test]
        public void should_return_null_when_type_is_not_in_assemblies()
        {
            ClassUnderTest = new GenericParser(new[] { typeof(String).Assembly });

            const string typeName = "FubuMVC.Spark.Tests.SparkModel.Binding.Generic<FubuMVC.Spark.Tests.SparkModel.Binding.Baz>";

            var type = ClassUnderTest.Parse(typeName);

            type.ShouldBeNull();
        }

        [Test]
        public void should_return_null_when_generic_argument_type_is_not_in_assemblies()
        {
            ClassUnderTest = new GenericParser(new[] { typeof(Bar).Assembly });

            const string typeName = "FubuMVC.Spark.Tests.SparkModel.Binding.Generic<FubuMVC.Spark.Tests.SparkModel.Binding.Baz, System.String>";

            var type = ClassUnderTest.Parse(typeName);

            type.ShouldBeNull();
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
        public void should_parse_a_mutiple_generic_arguments_across_different_assemblies()
        {
            const string typeName = "FubuMVC.Spark.Tests.SparkModel.Binding.Generic<FubuMVC.Spark.Tests.SparkModel.Binding.Baz, System.String>";

            var type = ClassUnderTest.Parse(typeName);

            type.ShouldEqual(typeof(Generic<Baz, string>));
        }

        [Test]
        public void should_detect_if_type_name_is_generic()
        {
            GenericParser.IsGeneric("System.String").ShouldBeFalse();
            GenericParser.IsGeneric("System.Collections.List<System.String>").ShouldBeTrue();
        }
    }
}