using System;
using System.Collections.Generic;
using System.Diagnostics;
using FubuMVC.Core.Assets;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetGraphScenarioTester
    {
        [Test]
        public void smoke_test_with_simple_request_1()
        {
            AssetGraphScenario.For("simple choice", @"
if the asset configuration is

requesting a.js, b.js, d.js, c.js
should return a.js, b.js, c.js, d.js
");
        }

        [Test]
        public void smoke_test_with_simple_request_2()
        {
            AssetGraphScenario.For("simple choice", @"
if the asset configuration is

requesting 
a.js 
b.js 
d.js 
c.js

should return 
a.js
b.js
c.js
d.js
");
        }


        [Test]
        public void smoke_test_that_reads_in_asset_dsl()
        {
            AssetGraphScenario.For("simple choice", @"
if the asset configuration is
core includes c.js, b.js, a.js

requesting core
should return a.js, b.js, c.js
");
        }

        [Test]
        public void smoke_test_that_should_fail()
        {
            Exception<ApplicationException>.ShouldBeThrownBy(() =>
            {
                AssetGraphScenario.For("simple choice", @"
if the asset configuration is
core includes c.js, b.js, a.js

requesting core
should return a.js, b.js
");
            });
        }


        [Test]
        public void Display_smoke_test_1()
        {
            var expected = new List<string>{"a", "a;lskdjf;ldskfj", "jdkfalskdf"};
            var actual = new List<string>(){
                "balskdfjsd",
                "a;lsdkjfl;kasdjf",
                "a;lskdfjkdsjflkasdjfl;ksdajflk;dskjf"
            };

            var display = new AssetGraphScenario.Display(expected, actual);

            Debug.WriteLine(display);
        }

        [Test]
        public void Display_smoke_test_2()
        {
            var expected = new List<string> { "a", "a;lskdjf;ldskfj", "jdkfalskdf", "asdlkf", "asldkfj.js" };
            var actual = new List<string>(){
                "balskdfjsd",
                "a;lsdkjfl;kasdjf",
                "a;lskdfjkdsjflkasdjfl;ksdajflk;dskjf"
            };

            var display = new AssetGraphScenario.Display(expected, actual);

            Debug.WriteLine(display);
        }

        [Test]
        public void Display_smoke_test_3()
        {
            var expected = new List<string> { "a", "a;lskdjf;ldskfj", "jdkfalskdf" };
            var actual = new List<string>(){
                "balskdfjsd",
                "a;lsdkjfl;kasdjf",
                "a;lskdfjkdsjflkasdjfl;ksdajflk;dskjf",
                "something",
                "extra"
            };

            var display = new AssetGraphScenario.Display(expected, actual);

            Debug.WriteLine(display);
        }
    }
}