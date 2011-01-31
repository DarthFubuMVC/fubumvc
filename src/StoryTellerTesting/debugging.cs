using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
            OperatorKeys.Keys.Each(x => Debug.WriteLine(x.Key));

        }
    }
}