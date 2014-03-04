using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Spark.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Binding
{
    [TestFixture]
    public class GenericViewModelTypeFinderTester : InteractionContext<GenericViewModelTypeFinder>
    {
        private Assembly[] _assemblies;

        protected override void beforeEach()
        {
            _assemblies = new[] {typeof (Bar).Assembly, typeof(List<>).Assembly};
        }

        [Test]
        public void should_parse_a_single_generic_argument()
        {
            const string typeName = "FubuMVC.Spark.Tests.SparkModel.Binding.Generic<FubuMVC.Spark.Tests.SparkModel.Binding.Baz>";

            var type = ClassUnderTest.Parse(typeName, _assemblies);

            type.ShouldEqual(typeof(Generic<Baz>));
        }

        [Test]
        public void should_parse_a_mutiple_generic_arguments()
        {
            const string typeName = "FubuMVC.Spark.Tests.SparkModel.Binding.Generic<FubuMVC.Spark.Tests.SparkModel.Binding.Baz, FubuMVC.Spark.Tests.SparkModel.Binding.Bar>";

            var type = ClassUnderTest.Parse(typeName, _assemblies);

            type.ShouldEqual(typeof(Generic<Baz, Bar>));
        }

        [Test]
        public void should_parse_a_mutiple_generic_arguments2()
        {
            const string typeName = "System.Collections.Generic.List`1<FubuMVC.Spark.Tests.SparkModel.Binding.Baz>";

            var type = ClassUnderTest.Parse(typeName, _assemblies);

            type.ShouldEqual(typeof(List<Baz>));
        }

        [Test]
        public void should_detect_if_type_name_is_generic()
        {
            GenericViewModelTypeFinder.IsGeneric("System.String").ShouldBeFalse();
            GenericViewModelTypeFinder.IsGeneric("System.Collections.List<System.String>").ShouldBeTrue();
        }
    }
}