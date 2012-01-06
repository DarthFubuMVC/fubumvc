using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.RazorModel.Scanning;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests.RazorModel.Scanning
{
    [TestFixture]
    public class RazorScannerTester : InteractionContext<FileScanner>
    {
        private IList<ITemplate> _scanResult;

        protected override void beforeEach()
        {
            Services.Inject<IFileSystem>(new FubuCore.FileSystem());
            _scanResult = new List<ITemplate>();
            var request = new ScanRequest();
            
			request.AddRoots(TestSource.Paths());
			request.IncludeRazorViews();
            request.AddHandler(file => _scanResult.Add(new Template(file.Path, file.Root, "")));

            ClassUnderTest.Scan(request);
        }

        [Test]
        public void all_razor_files_in_sources_are_found()
        {
            _scanResult.ShouldHaveCount(49);
        }

        [Test]
        public void correct_root_is_assigned_to_found_files()
        {
            Func<string, string> pathFor = root => TestSource.Paths().Single(p => p.EndsWith(root));

            _scanResult.Where(s => s.RootPath == pathFor("Templates")).ShouldHaveCount(37);
            _scanResult.Where(s => s.RootPath == pathFor("Pak1")).ShouldHaveCount(8);
            _scanResult.Where(s => s.RootPath == pathFor("Pak2")).ShouldHaveCount(4);
        }
    }

    public static class TestSource 
    {
        public static IEnumerable<string> Paths()
        {
            var templatePath = FubuCore.FileSystem.Combine(Directory.GetCurrentDirectory(), "Templates");
            yield return FubuCore.FileSystem.Combine(templatePath, "Pak1");
            yield return FubuCore.FileSystem.Combine(templatePath, "Pak2");
            yield return templatePath;
        }
    }
}