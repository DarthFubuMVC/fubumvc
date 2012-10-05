using System;
using System.Diagnostics;
using System.Threading;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Core.Assets.IntegrationTesting
{
    [TestFixture]
    public class file_watching_changes_the_asset : FubuRegistryHarness
    {
        private string _file;
        private string _originalText;
        private string _newText;

        protected override void beforeRunning()
        {
            FubuMode.Mode(FubuMode.Development);

            _file = Harness.GetApplicationDirectory().AppendPath("content", "scripts", "ignoreme.js");

            _originalText = Guid.NewGuid().ToString();
            _newText = Guid.NewGuid().ToString();

            new FileSystem().WriteStringToFile(_file, _originalText);
        }

        [Test]
        public void read_asset_file_then_change_it_and_read_it_again()
        {
            var text1 = endpoints.GetAsset(AssetFolder.scripts, "ignoreme.js").ReadAsText();
            Debug.WriteLine(text1);

            text1.ShouldEqual(_originalText);

            new FileSystem().WriteStringToFile(_file, _newText);

            // Yes, I really did this.  The scanning for changes polls on 5 second intervals and I'm too
            // lazy to do something smarter than this this morning.  Not too lazy to write a comment
            // justifying this little bit of slop, but too lazy to do it right
            Thread.Sleep(10000);

            var text2 = endpoints.GetAsset(AssetFolder.scripts, "ignoreme.js").ReadAsText();
            Debug.WriteLine(text2);
            text2.ShouldEqual(_newText);
        }

        protected override void afterRunning()
        {
            new FileSystem().DeleteFile(_file);

            FubuMode.Reset();
        }
    }
}