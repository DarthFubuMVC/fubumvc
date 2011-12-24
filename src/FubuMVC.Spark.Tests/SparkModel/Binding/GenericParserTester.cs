using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
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
            var duplicatedGenericExtraAssembly = generateAssembly("namespace FubuMVC.Spark.Tests.SparkModel.Binding{public class DuplicatedGeneric<T>{} public class Duplicated{} }");

            ClassUnderTest = new GenericParser(new[] { typeof(Bar).Assembly, typeof(String).Assembly, duplicatedGenericExtraAssembly });
        }

        private static Assembly generateAssembly(string source)
        {
            var parms = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false
            };

            return CodeDomProvider
                .CreateProvider("CSharp")
                .CompileAssemblyFromSource(parms, source)
                .CompiledAssembly;
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

        [Test]
        public void finding_open_type_should_add_error_when_no_matching_type_is_found()
        {
            var typeDefinition = new GenericTypeDefinition {OpenTypeName = "NotFound", ArgumentTypeNames = new[] {"System.String"}};

            var result = ClassUnderTest.findOpenType(typeDefinition);

            result.ShouldBeNull();
            ClassUnderTest.ParseErrors.First().ShouldContain("No generic type matching");
        }

        [Test]
        public void finding_open_type_should_add_error_when_multiple_matching_types_are_found()
        {
            var typeDefinition = new GenericTypeDefinition { OpenTypeName = "FubuMVC.Spark.Tests.SparkModel.Binding.DuplicatedGeneric`1", ArgumentTypeNames = new[] { "System.String" } };

            var result = ClassUnderTest.findOpenType(typeDefinition);

            result.ShouldBeNull();
            ClassUnderTest.ParseErrors.First().ShouldContain("More than one generic types matching");
        }

        [Test]
        public void finding_generic_arguments_should_add_error_when_no_matching_type_is_found()
        {
            var typeDefinition = new GenericTypeDefinition { OpenTypeName = "FubuMVC.Spark.Tests.SparkModel.Binding.Generic`1", ArgumentTypeNames = new[] { "NOTFOUND" } };

            var result = ClassUnderTest.findGenericArgumentTypes(typeDefinition);

            result.ShouldBeNull();
            ClassUnderTest.ParseErrors.First().ShouldContain("No generic argument type matching");
        }

        [Test]
        public void finding_generic_arguments_should_add_error_when_multiple_matching_types_are_found()
        {
            var typeDefinition = new GenericTypeDefinition { OpenTypeName = "FubuMVC.Spark.Tests.SparkModel.Binding.Generic`1", ArgumentTypeNames = new[] { "FubuMVC.Spark.Tests.SparkModel.Binding.Duplicated" } };

            var result = ClassUnderTest.findGenericArgumentTypes(typeDefinition);

            result.ShouldBeNull();
            ClassUnderTest.ParseErrors.First().ShouldContain("More than one generic argument types matching");
        }

        [Test]
        public void if_type_is_not_generic_returns_null()
        {
            ClassUnderTest.Parse("System.String").ShouldBeNull();
        }

        [Test]
        public void if_type_is_not_generic_then_register_error()
        {
            ClassUnderTest.Parse("System.String");
            ClassUnderTest.ParseErrors.ShouldHaveCount(1).First().ShouldEqual("Type System.String does not appear to be generic.");
        }
    }

    public class Duplicated
    {

    }
}