using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
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
    }
}