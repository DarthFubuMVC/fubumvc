﻿using System;
using Shouldly;
using Xunit;
using FubuMVC.Core;

namespace FubuMVC.Tests
{
    
    public class UrlStringExtensionsTester
    {
        [Fact]
        public void to_absolute_url_when_it_is_already_an_absolute_url()
        {
            "http://cnn.com".ToAbsoluteUrl("http://localhost:5050")
                        .ShouldBe("http://cnn.com");
        }

        [Fact]
        public void to_absolute_url_from_relative_to_absolute()
        {

            "/foo".ToAbsoluteUrl("http://localhost:5050")
                  .ShouldBe("http://localhost:5050/foo");
        }


        [Fact]
        public void to_absolute_url_from_relative_to_absolute_2()
        {
            "~/foo".ToAbsoluteUrl("http://localhost:5050")
                  .ShouldBe("http://localhost:5050/foo");
        }

        [Fact]
        public void to_absolute_url_respects_the_template()
        {
            "http://cnn.com/{script}".ToAbsoluteUrl("http://localhost:5050")
                                     .ShouldBe("http://cnn.com/{script}");
        }

        [Fact]
        public void url_encoding()
        {
            "abc/def&ghi=jkl".UrlEncoded().ShouldBe("abc%2Fdef%26ghi%3Djkl");
        }
    }
}