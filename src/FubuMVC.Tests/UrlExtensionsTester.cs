using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class UrlExtensionsTester
    {
        public class TestObject
        {
            public TestObject Child { get; set; }
            public int Value { get; set; }
            public bool Boolean { get; set; }
        }

        [Test]
        public void UrlEncode_should_encode_string()
        {
            string test = "encode test";

            test.UrlEncoded().ShouldBe("encode%20test");
        }
    }
}