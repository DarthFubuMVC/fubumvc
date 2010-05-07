using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class LocalizationManagerTester
    {
        [Test]
        public void get_text_for_key_should_return_key() //NOTE: just a reminder
        {
            LocalizationManager.GetTextForKey("key").ShouldEqual("key");
        }
    }
}