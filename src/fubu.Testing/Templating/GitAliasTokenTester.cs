using Fubu.Templating;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Templating
{
    [TestFixture]
    public class GitAliasTokenTester
    {
        [Test]
        public void should_equal_token_with_same_name()
        {
            var token1 = new GitAliasToken {Name = "test"};
            var token2 = new GitAliasToken { Name = "test" };

            token1.ShouldEqual(token2);
        }
    }
}