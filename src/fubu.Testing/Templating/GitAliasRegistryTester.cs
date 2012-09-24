using Fubu.Templating;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Templating
{
    [TestFixture]
    public class GitAliasRegistryTester
    {
        [Test]
        public void should_setup_defaults()
        {
            var registry = new GitAliasRegistry();
            shouldSetupDefaults(registry);
        }

        [Test]
        public void should_setup_defaults_after_values_are_set()
        {
            var registry = new GitAliasRegistry();
            registry.Aliases = new[] { new GitAliasToken { Name = "Test", Url = "test"} };

            registry.AliasFor("Test").ShouldNotBeNull();
            shouldSetupDefaults(registry);
        }

        private void shouldSetupDefaults(GitAliasRegistry registry)
        {
            registry
                .AliasFor("fubusln")
                .Url
                .ShouldEqual("git://github.com/DarthFubuMVC/rippletemplate.git");

            registry
                .AliasFor("fububottle")
                .Url
                .ShouldEqual("git://github.com/DarthFubuMVC/bottle-template.git");
        }
    }
}