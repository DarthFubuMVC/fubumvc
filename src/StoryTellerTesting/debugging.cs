using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using System.Collections.Generic;

namespace IntegrationTesting
{
    [TestFixture, Explicit]
    public class debugging
    {
        [Test]
        public void TESTNAME()
        {
            var assembly = typeof (JqGridModel).Assembly;
            assembly.GetManifestResourceNames().Each(x =>
            {
                var info = assembly.GetManifestResourceInfo(x);

                var stream = assembly.GetManifestResourceStream(x);
                var reader = new StreamReader(stream);
                Debug.WriteLine(reader.ReadToEnd());


                Debug.WriteLine(info.FileName);
            });


            var i = assembly.GetManifestResourceInfo("JScript1.js");
            Debug.WriteLine(i.FileName);
        }

        [Test]
        public void serialize_a_package_manifest()
        {
            var manifest = new PackageManifest();
            manifest.Name = "something";
            manifest.AddAssembly("A");
            manifest.AddAssembly("B");

            var serializer = new XmlSerializer(typeof (PackageManifest));
            var writer = new StringWriter();
            
            serializer.Serialize(writer, manifest);

            Debug.WriteLine(writer.ToString());
        }
    }
}