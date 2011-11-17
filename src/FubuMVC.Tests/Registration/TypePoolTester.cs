using System;
using System.CodeDom.Compiler;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class TypePoolTester
    {
        [Test]
        public void should_have_the_calling_assembly_by_default()
        {
            var pool = new TypePool(GetType().Assembly);
            pool.Assemblies.ShouldContain(GetType().Assembly);
        }

        [Test]
        public void removing_the_called_assembly()
        {
            var pool = new TypePool(GetType().Assembly);
            pool.IgnoreCallingAssembly();

            pool.Assemblies.ShouldNotContain(GetType().Assembly);
        }

        [Test]
        public void should_return_all_types_with_full_name()
        {
            var pool = new TypePool(GetType().Assembly);
            
            pool.AddType(generateType("namespace FubuMVC.Core{public class Endpoint{}}", "FubuMVC.Core.Endpoint"));
            pool.AddType(typeof(Core.Endpoint));

            pool.TypesWithFullName(typeof(Core.Endpoint).FullName).ShouldHaveCount(2);
        }

        private static Type generateType(string source, string fullName)
        {
            var parms = new CompilerParameters {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false
            };

            return CodeDomProvider
                .CreateProvider("CSharp")
                .CompileAssemblyFromSource(parms, source)
                .CompiledAssembly
                .GetType(fullName);
        } 
    }
}