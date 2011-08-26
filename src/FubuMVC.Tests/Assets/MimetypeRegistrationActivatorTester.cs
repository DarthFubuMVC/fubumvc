using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Assets.Content;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class MimetypeRegistrationActivatorTester
    {
        private ITransformerPolicy[] thePolicies;
        private PackageLog theLog;


        [TestFixtureSetUp]
        public void SetUp()
        {
            thePolicies = new ITransformerPolicy[]{
                new GlobalTransformerPolicy<CoffeeTransformer>(MimeType.Javascript, BatchBehavior.NoBatching, ".coffee"),
                new JavascriptTransformerPolicy<StubTransformer>(ActionType.Transformation, ".a", ".b"),
                new CssTransformerPolicy<ATransformer>(ActionType.Substitution, ".c", ".d")
            };

            theLog = new PackageLog();

            new MimetypeRegistrationActivator(thePolicies).Activate(new IPackageInfo[0],theLog);

            
        }

        [Test]
        public void should_have_made_the_connections_to_the_mime_types()
        {
            MimeType.Javascript.HasExtension(".coffee").ShouldBeTrue();
            MimeType.Javascript.HasExtension(".a").ShouldBeTrue();
            MimeType.Javascript.HasExtension(".b").ShouldBeTrue();
            MimeType.Javascript.HasExtension(".c").ShouldBeFalse();  // Just seeing if you're paying attention

            MimeType.Css.HasExtension(".c").ShouldBeTrue();
            MimeType.Css.HasExtension(".d").ShouldBeTrue();


            
        }

        [Test]
        public void did_some_logging_too()
        {
            theLog.FullTraceText().ShouldContain("Registered extension .coffee with MimeType " + MimeType.Javascript.Value);
        }

    }
}