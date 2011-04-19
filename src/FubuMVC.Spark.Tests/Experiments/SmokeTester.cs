using System;
using System.IO;
using FubuMVC.Spark.Tokenization;
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
            var view = Path.Combine(outputPath, "Templates", "A3.spark");
            var master = Path.Combine(outputPath, "Templates", "Shared", "application.spark");

            var item = new SparkItem(view, outputPath, "")
            {
                Master = new SparkItem(master, outputPath, "")
            };

            // We could do an extension method, ToDescriptor() - however, a bit tricky if master is from host and package is outside, dev mode.. 
            // Seems that we need to handle registrations differently (combined view folders for packages + host)
            // Each package gets a virtual path set in a activator by fubu, VirtualPathProviderActivator.
            // Perhas we can utilize this, we have the origin (package names)..
            
            var descriptor = new SparkViewDescriptor();
            descriptor.AddTemplate(item.RelativePath());
            if(item.Master != null)
            {                
                descriptor.AddTemplate(item.Master.RelativePath());                
            }

            var engine = new SparkViewEngine
            {
                ViewFolder = new FileSystemViewFolder(item.Root)
            };

            var entry = engine.CreateEntry(descriptor);
            var instance = entry.CreateInstance();

            var writer = new StringWriter();
            instance.RenderView(writer);
            var expected = string.Format("<div>Host Application: this is the header{0}hello world{0}this is the footer</div>", Environment.NewLine);
            Assert.AreEqual(expected, writer.ToString());
        }
    }
}