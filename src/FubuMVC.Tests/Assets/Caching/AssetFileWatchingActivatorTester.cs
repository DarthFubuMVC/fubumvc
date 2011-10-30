using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets.Caching;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Caching
{
    [TestFixture]
    public class AssetFileWatchingActivatorTester : InteractionContext<AssetFileWatchingActivator>
    {
        [Test]
        public void activate_simply_starts_the_asset_file_watcher_going()
        {
            ClassUnderTest.Activate(new IPackageInfo[0], new PackageLog());

            MockFor<IAssetFileWatcher>().AssertWasCalled(x => x.StartWatchingAll());
        }
    }
}