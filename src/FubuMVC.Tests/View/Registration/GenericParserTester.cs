using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.View.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.View.Registration
{
    [TestFixture]
    public class GenericParserTester
    {
        private GenericParser ClassUnderTest;

        [SetUp]
        public void beforeEach()
        {
            var duplicatedGenericExtraAssembly = generateAssembly("namespace FubuMVC.Tests.View.Registration{public class DuplicatedGeneric<T>{} public class Duplicated{} }");

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

            const string typeName = "FubuMVC.Tests.View.Registration.Generic<FubuMVC.Tests.View.Registration.Baz>";

            var type = ClassUnderTest.Parse(typeName);

            type.ShouldBeNull();
        }

        [Test]
        public void should_return_null_when_generic_argument_type_is_not_in_assemblies()
        {
            ClassUnderTest = new GenericParser(new[] { typeof(Bar).Assembly });

            const string typeName = "FubuMVC.Tests.View.Registration.Generic<FubuMVC.Tests.View.Registration.Baz, System.String>";

            var type = ClassUnderTest.Parse(typeName);

            type.ShouldBeNull();
        }

        [Test]
        public void should_parse_a_single_generic_argument()
        {
            const string typeName = "FubuMVC.Tests.View.Registration.Generic<FubuMVC.Tests.View.Registration.Baz>";

            var type = ClassUnderTest.Parse(typeName);

            type.ShouldEqual(typeof(Generic<Baz>));
        }

        [Test]
        public void should_parse_a_mutiple_generic_arguments()
        {
            const string typeName = "FubuMVC.Tests.View.Registration.Generic<FubuMVC.Tests.View.Registration.Baz, FubuMVC.Tests.View.Registration.Bar>";

            var type = ClassUnderTest.Parse(typeName);

            type.ShouldEqual(typeof(Generic<Baz, Bar>));
        }

        [Test]
        public void should_parse_a_mutiple_generic_arguments_across_different_assemblies()
        {
            const string typeName = "FubuMVC.Tests.View.Registration.Generic<FubuMVC.Tests.View.Registration.Baz, System.String>";

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
            var typeDefinition = new GenericTypeDefinition { OpenTypeName = "FubuMVC.Tests.View.Registration.DuplicatedGeneric`1", ArgumentTypeNames = new[] { "System.String" } };

            var result = ClassUnderTest.findOpenType(typeDefinition);

            result.ShouldBeNull();
            ClassUnderTest.ParseErrors.First().ShouldContain("More than one generic types matching");
        }

        [Test]
        public void finding_generic_arguments_should_add_error_when_no_matching_type_is_found()
        {
            var typeDefinition = new GenericTypeDefinition { OpenTypeName = "FubuMVC.Tests.View.Registration.Generic`1", ArgumentTypeNames = new[] { "NOTFOUND" } };

            var result = ClassUnderTest.findGenericArgumentTypes(typeDefinition);

            result.ShouldBeNull();
            ClassUnderTest.ParseErrors.First().ShouldContain("No generic argument type matching");
        }

        [Test]
        public void finding_generic_arguments_should_add_error_when_multiple_matching_types_are_found()
        {
            var typeDefinition = new GenericTypeDefinition { OpenTypeName = "FubuMVC.Tests.View.Registration.Generic`1", ArgumentTypeNames = new[] { "FubuMVC.Tests.View.Registration.Duplicated" } };

            var result = ClassUnderTest.findGenericArgumentTypes(typeDefinition);

            result.ShouldBeNull();
            ClassUnderTest.ParseErrors.First().ShouldContain("More than one generic argument types matching");
        }

    }

    public class Bar{}

    public class Baz
    {
    }

    public class Generic<T>{}
    public class DuplicatedGeneric<T>{}
    public class Generic<T1, T2>{}

    public class Duplicated
    {

    }
}