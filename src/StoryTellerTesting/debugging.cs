using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.WebForms;
using NUnit.Framework;
using System.Collections.Generic;
using FubuTestingSupport;
using TestPackage2;

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
        public void load_test_package2()
        {
            var registry = new TestPackage2Registry();
            var graph = registry.BuildGraph();

            graph.BehaviorFor<StringController>(x => x.SayHello()).ShouldNotBeNull();
        }

    }


}