using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetGraphConfigurationActivatorBehaviorTester : InteractionContext<AssetGraphConfigurationActivator>
    {
        IPackageLog _log;
        IFileSystem _fileSystem;
        AssetGraph _assetGraph;

        protected override void beforeEach()
        {
            _log = MockFor<IPackageLog>();
            _fileSystem = MockFor<IFileSystem>();
            _assetGraph = MockFor<AssetGraph>();
            _fileSystem.Expect(x => x.FindFiles(Arg<string>.Is.Anything, Arg<FileSet>.Is.Anything))
                .Return(new string[] { });
        }

        [Test]
        public void calling_activate_invokes_any_registered_asset_precompilers()
        {
            var result = false;
            _assetGraph.OnPrecompile(x =>
            {
                result = true;
            });

            ClassUnderTest.Activate(new IPackageInfo[] { }, _log);
            result.ShouldBeTrue();
        }
    }
}