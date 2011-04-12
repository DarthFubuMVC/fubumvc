using System;
using System.IO;
using FubuMVC.Spark.Scanning;
using NUnit.Framework;
using Spark;
using Spark.FileSystem;

namespace FubuMVC.Spark.Tests.Experiments
{
    public class SmokeTester
    {
        [Test]
        public void smoke()
        {
            var outputPath = AppDomain.CurrentDomain.BaseDirectory;

            var filePath = Path.Combine(outputPath, "Scanning", "Templates", "A3.spark");

            var file = new SparkFile(filePath, outputPath, "");

            var engine = new SparkViewEngine();
            var descriptor = new SparkViewDescriptor();
            var templates = new[]
            {
                // view path
                file.RelativePath(),
                // master page
                // NOT NECESSARILY THIS WAY
                // WE COULD PARSE FOR <use master="application.spark|something.spark"/> AND SET THIS IN THE DESCRIPTOR
                // WE COULD USE A CONVENTION TO SET THE MASTER PAGE BY ACTION CALL
                // OR SIMPLY NO MASTER PAGE (partials)
                // IN THE END, EACH ONE OF THOSE (OR ANY OTHER) IDEALLY SHOULD BECOME AN STRATEGY
                Path.Combine("Scanning", "Templates", "Shared", "application.spark")
            };
            engine.ViewFolder = new FileSystemViewFolder(file.Root);
            foreach (var template in templates)
            {
                descriptor.AddTemplate(template);
            }
            var entry = engine.CreateEntry(descriptor);
            var instance = entry.CreateInstance();
            var writer = new StringWriter();
            instance.RenderView(writer);
            var expected = string.Format("<div>this is the header{0}hello world{0}this is the footer{0}</div>", Environment.NewLine);
            Assert.AreEqual(expected, writer.ToString());
        }
    }
}