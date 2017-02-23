using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests
{
    
    public class UrlExtensionsTester
    {
        public class TestObject
        {
            public TestObject Child { get; set; }
            public int Value { get; set; }
            public bool Boolean { get; set; }
        }

        [Fact]
        public void UrlEncode_should_encode_string()
        {
            string test = "encode test";

            test.UrlEncoded().ShouldBe("encode%20test");
        }
    }
}