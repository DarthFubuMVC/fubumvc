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
            using (FubuRuntime.Basic(_ => _.Features.Localization.Enable(true))) ;
        }
    }
}