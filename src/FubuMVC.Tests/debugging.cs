using System.Diagnostics;
using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class debugging
    {
        [Test]
        public void try_configuration()
        {
            var registry = new FubuRegistry();

            var config = registry.Configuration;
            Debug.WriteLine(config);
        }
    }
}