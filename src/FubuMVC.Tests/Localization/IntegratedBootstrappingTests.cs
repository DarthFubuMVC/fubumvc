using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.Tests.Localization
{
    [TestFixture]
    public class IntegratedBootstrappingTests
    {
        [Test]
        public void smoke()
        {
            new LocalizationApplication()
                .BuildApplication()
                .Bootstrap();
        }

        public class LocalizationApplication : IApplicationSource
        {
            public FubuApplication BuildApplication(string directory = null)
            {
                return FubuApplication
                    .For<LocalizationRegistry>();
            }
        }
        public class LocalizationRegistry : FubuRegistry
        {
            public LocalizationRegistry()
            {
                Features.Localization.Enable(true);
            }
        }
    }
}