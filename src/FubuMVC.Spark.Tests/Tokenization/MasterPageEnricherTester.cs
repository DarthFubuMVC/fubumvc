using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.Tokenization;
using FubuMVC.Spark.Tokenization.Model;
using FubuMVC.Spark.Tokenization.Parsing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Tokenization
{
    public class MasterPageEnricherTester : InteractionContext<MasterPageEnricher>
    {
        private EnrichmentContext _context;
        private SparkFiles _sparkFiles;

        const string Host = "host";
        const string Pak1 = "pak1";
        const string Pak2 = "pak2";
        const string Pak3 = "pak3";

        private readonly string _hostRoot;
        private readonly string _pak1Root;
        private readonly string _pak2Root;
        private readonly string _pak3Root;

        public MasterPageEnricherTester()
        {
            _hostRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inetpub", "www", "web");
            _pak1Root = Path.Combine(_hostRoot, Pak1);
            _pak2Root = Path.Combine(_hostRoot, Pak2);
            _pak3Root = Path.Combine(_hostRoot, Pak3);            
        }

        protected override void beforeEach()
        {
            _context = new EnrichmentContext {SparkFiles = _sparkFiles = createFiles()};

            MockFor<ISparkParser>()
                .Stub(x => x.Parse(_context.FileContent, "use", "master")).Return("application");
        }

        private SparkFiles createFiles()
        {
            return new SparkFiles
            {
                newSpark(_pak1Root, Pak1, "Actions", "Controllers", "Home", "Home.spark"),
                newSpark(_pak1Root, Pak1, "Actions", "Handlers", "Products", "list.spark"),
                newSpark(_pak1Root, Pak1, "Actions", "Shared", "application.spark"),
                newSpark(_pak2Root, Pak2, "Features", "Controllers", "Home", "Home.spark"),
                newSpark(_pak2Root, Pak2, "Features", "Handlers", "Products", "list.spark"),
                newSpark(_pak2Root, Pak2, "Shared", "application.spark"),
                
                newSpark(_pak3Root, Pak3, "Features", "Controllers", "Home", "Home.spark"),

                newSpark(_hostRoot, Host, "Actions", "Shared", "application.spark"),
                newSpark(_hostRoot, Host, "Features", "Mixer", "chuck.spark"),
                newSpark(_hostRoot, Host, "Features", "Mixer", "Shared", "application.spark"),                
                newSpark(_hostRoot, Host, "Features", "roundkick.spark"),
                newSpark(_hostRoot, Host, "Handlers", "Products", "details.spark"),
                newSpark(_hostRoot, Host, "Shared", "application.spark")
            };
        }

        private SparkFile newSpark(string root, string origin, params string[] relativePaths)
        {
            var paths = new[]{root}.Union(relativePaths).ToArray();
            return new SparkFile(FileSystem.Combine(paths), root, origin);
        }

        [Test]
        public void master_is_the_closest_ancestor_with_the_specified_name_in_shared_1()
        {
            var sparkFile = _sparkFiles.First();
            ClassUnderTest.Enrich(sparkFile, _context);
            _sparkFiles.ElementAt(2).ShouldEqual(sparkFile.Master);
        }

        [Test]
        public void master_is_the_closest_ancestor_with_the_specified_name_in_shared_2()
        {
            var sparkFile = _sparkFiles.ElementAt(3);
            ClassUnderTest.Enrich(sparkFile, _context);
            _sparkFiles.ElementAt(5).ShouldEqual(sparkFile.Master);
        }

        [Test]
        public void fallback_to_master_in_shared_host_when_no_local_ancestor_exists()
        {
            var sparkFile = _sparkFiles.ElementAt(6);
            ClassUnderTest.Enrich(sparkFile, _context);
            _sparkFiles.Last().ShouldEqual(sparkFile.Master);
        }

        [Test]
        public void fallback_to_master_in_host_1()
        {
            var sparkFile = _sparkFiles.ElementAt(8);
            ClassUnderTest.Enrich(sparkFile, _context);
            _sparkFiles.ElementAt(9).ShouldEqual(sparkFile.Master);
        }

        [Test]
        public void fallback_to_master_in_host_2()
        {
            var sparkFile = _sparkFiles.ElementAt(10);
            ClassUnderTest.Enrich(sparkFile, _context);
            _sparkFiles.Last().ShouldEqual(sparkFile.Master);
        }

        // TODO : Edge cases, boundaries, more tests for expected behaviors
    }
}