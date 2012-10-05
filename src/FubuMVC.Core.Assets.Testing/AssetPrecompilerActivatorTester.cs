using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets
{
    public class AssetPrecompilerActivatorTester : InteractionContext<AssetPrecompilerActivator>
    {
        AssetGraph _assetGraph;
        IAssetPrecompiler _precompiler;
        IAssetFileGraph _assetFileGraph;
        IPackageLog _log;
        bool _result;

        protected override void beforeEach()
        {
            _assetGraph = MockFor<AssetGraph>();
            _precompiler = MockFor<IAssetPrecompiler>();
            _assetFileGraph = MockFor<IAssetFileGraph>();
            _precompiler
                .Expect(x => x.Precompile(Arg<IAssetFileGraph>.Is.Same(_assetFileGraph), Arg<IAssetRegistration>.Is.Same(_assetGraph)))
                .Callback<IAssetFileGraph, IAssetRegistration>((x, y) => _result = true);
            _log = MockFor<IPackageLog>();
            Services.Inject<IEnumerable<IAssetPrecompiler>>(new[] { _precompiler });
        }

        [Test]
        public void calling_activate_registers_all_asset_precompilers_with_the_asset_graph()
        {
            ClassUnderTest.Activate(new IPackageInfo[] { }, _log);

            _assetGraph.Precompile();

            _result.ShouldBeTrue();
        }
    }
}