using FubuMVC.Core.Registration;
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
    }
}