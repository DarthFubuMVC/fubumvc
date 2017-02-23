﻿using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.View;
using HtmlTags;
using Xunit;

namespace FubuMVC.IntegrationTesting.Assets
{
    
    public class Tag_building_in_development_mode : AssetIntegrationContext
    {
        public Tag_building_in_development_mode()
        {
            Mode = "development";

            File("content/scripts/script1.js");

            File("content/styles/styles1.css");

            File("content/images/image1.bmp");
        }


        [Fact]
        public void request_an_image_that_cannot_be_found()
        {
            Scenario.Get.Action<MissingAssetsEndpoint>(x => x.get_missing_image());

            Scenario.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
            Scenario.ContentShouldContain("Requested image 'missing_image.bmp' cannot be found".HtmlEncode());
        }

        [Fact]
        public void request_a_script_that_cannot_be_found()
        {
            Scenario.Get.Action<MissingAssetsEndpoint>(x => x.get_missing_script());

            Scenario.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
            Scenario.ContentShouldContain(
                "Requested script(s) 'nonexistent.js', 'random.js' cannot be found".HtmlEncode());
        }

        [Fact]
        public void request_a_stylesheet_that_cannot_be_found()
        {
            Scenario.Get.Action<MissingAssetsEndpoint>(x => x.get_missing_stylesheet());

            Scenario.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
            Scenario.ContentShouldContain(
                "Requested stylesheets(s) 'nonexistent.css', 'weird.css' cannot be found".HtmlEncode());
        }
    }

    public class MissingAssetsEndpoint
    {
        private readonly FubuHtmlDocument _document;

        public MissingAssetsEndpoint(FubuHtmlDocument document)
        {
            _document = document;
        }

        public HtmlDocument get_missing_image()
        {
            _document.Add(x => x.Image("missing_image.bmp"));

            return _document;
        }

        public HtmlDocument get_missing_script()
        {
            _document.Add(x => x.Script("nonexistent.js", "random.js", "script1.js"));

            return _document;
        }

        public HtmlDocument get_missing_stylesheet()
        {
            _document.Head.Append(_document.Css("nonexistent.css", "weird.css"));

            return _document;
        }
    }
}