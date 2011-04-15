using System;
using System.IO;
using System.Linq;
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

        protected override void beforeEach()
        {
            _context = new EnrichmentContext {SparkFiles = _sparkFiles = createFiles()};

            MockFor<ISparkParser>()
                .Stub(x => x.Parse(_context.FileContent, "use", "master")).Return("application");
        }

        private SparkFiles createFiles()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var root = Path.Combine(baseDirectory, "inetput", "www", "web");
            return new SparkFiles
            {
                new SparkFile(Path.Combine(root, "Controllers", "Home", "Home.spark"), root,"Root"),
                new SparkFile(Path.Combine(root, "Handlers", "Products", "list.spark"),root, "Root"),
                new SparkFile(Path.Combine(root, "Actions", "Shared", "application.spark"),root, "Root"),
                new SparkFile(Path.Combine(root, "Handlers", "Shared", "application.spark"),root, "Root"),
                new SparkFile(Path.Combine(root, "Controllers", "Shared", "application.spark"),root, "Root"),
                new SparkFile(Path.Combine(root, "Shared", "application.spark"),root, "Root"),
                new SparkFile(Path.Combine(root, "Handlers", "Products", "details.spark"),root, "Root")
            };
        }

        [Test]
        public void master_is_the_nearest_file_with_the_same_name_in_shared()
        {
            var sparkFile = _sparkFiles.First();
            ClassUnderTest.Enrich(sparkFile, _context);
            _sparkFiles.ElementAt(4).ShouldEqual(sparkFile.Master);
        }
    }
}