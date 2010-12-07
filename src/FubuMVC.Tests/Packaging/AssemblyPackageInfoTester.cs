using System.Reflection;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class AssemblyPackageInfoTester
    {
        private Assembly assembly;
        private AssemblyPackageInfo package;

        [SetUp]
        public void SetUp()
        {
            assembly = Assembly.GetExecutingAssembly();
            package = new AssemblyPackageInfo(assembly);
        }

        [Test]
        public void name_just_returns_the_assembly_name()
        {
            package.Name.ShouldEqual("Assembly:  " + assembly.GetName().FullName);
        }

        [Test]
        public void load_assemblies_just_tries_to_add_the_inner_assembly_directly()
        {
            var loader = MockRepository.GenerateMock<IAssemblyRegistration>();
            package.LoadAssemblies(loader);

            loader.AssertWasCalled(x => x.Use(assembly));
        }
    }
}