using FubuCore.Binding;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ServiceArgumentsTester
    {
        public class Plugin{}

        [Test]
        public void set_should_add_plugin()
        {
            var args = new ServiceArguments();
            var plugin = new Plugin();
            args.Has(typeof(Plugin)).ShouldBeFalse();
            args.Set(typeof(Plugin), plugin);
            args.Has(typeof(Plugin)).ShouldBeTrue();
        }
    }
}