using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.UI;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Assets
{
    [TestFixture]
    public class Tag_building_in_production_mode : AssetIntegrationContext
    {
        public Tag_building_in_production_mode()
        {
            FubuMode.Reset();

            File("content/scripts/script1.js");

            File("content/styles/styles1.css");

            File("content/images/image1.bmp");

        }

        [Test]
        public void run_with_existing_assets()
        {
            Scenario.Get.Action<TagBuildingEndpoint>(x => x.get_page_with_assets());

            Scenario.ContentShouldContain("<link href=\"/content/styles/styles1.css\" rel=\"stylesheet\" type=\"text/css\" />");
            Scenario.ContentShouldContain("<script type=\"text/javascript\" src=\"/content/scripts/script1.js\">");
            Scenario.ContentShouldContain("<img src=\"/content/images/image1.bmp\" />");

            Scenario.StatusCodeShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void non_existent_assets_show_up_with_the_supplied_url()
        {
            Scenario.Get.Action<TagBuildingEndpoint>(x => x.get_page_with_assets());

            Scenario.ContentShouldContain("</script><script type=\"text/javascript\" src=\"/nonexistent.js\"></script>");
            Scenario.ContentShouldContain("<link href=\"/nonexistent.css\" rel=\"stylesheet\" type=\"text/css\" />");

            Scenario.ContentShouldContain("<img src=\"/image2.bmp\" />");
        }

        [Test]
        public void get_page_with_optional_assets()
        {
            Scenario.Get.Action<TagBuildingEndpoint>(x => x.get_page_with_optional_assets());
            Scenario.StatusCodeShouldBe(HttpStatusCode.OK);

            // these exist and should be written out
            Scenario.ContentShouldContain("<link href=\"/content/styles/styles1.css\" rel=\"stylesheet\" type=\"text/css\" />");
            Scenario.ContentShouldContain("<script type=\"text/javascript\" src=\"/content/scripts/script1.js\">");

            // these don't, so don't do anything with them
            Scenario.ContentShouldNotContain("</script><script type=\"text/javascript\" src=\"nonexistent.js\"></script>");
            Scenario.ContentShouldNotContain("<link href=\"nonexistent.css\" rel=\"stylesheet\" type=\"text/css\" />");

        }
    }

    public class TagBuildingEndpoint
    {
        private readonly FubuHtmlDocument _document;

        public TagBuildingEndpoint(FubuHtmlDocument document)
        {
            _document = document;
        }

        public HtmlDocument get_page_with_assets()
        {
            _document.Head.Append(_document.Css("styles1.css", "nonexistent.css"));

            _document.Add(x => x.Image("image1.bmp"));
            _document.Add(x => x.Image("image2.bmp"));

            _document.Push("footer");
            _document.Add(x => x.Script("script1.js", "nonexistent.js"));

            return _document;
        }




        public HtmlDocument get_page_with_optional_assets()
        {
            _document.Head.Append(_document.OptionalCss("styles1.css", "nonexistent.css"));


            _document.Push("footer");
            _document.Add(x => x.OptionalScript("script1.js", "nonexistent.js"));

            return _document;
        }

    }
}