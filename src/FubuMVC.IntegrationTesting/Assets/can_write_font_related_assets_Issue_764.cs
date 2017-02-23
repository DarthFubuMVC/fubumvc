using Xunit;

namespace FubuMVC.IntegrationTesting.Assets
{
    
    public class can_write_font_related_assets_Issue_764
    {
        [Fact]
        public void read_WOFF_file()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("content/styles/fonts/286692_2_0.woff");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/font-woff");
            });
        }

        [Fact]
        public void read_WOFF2_file()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("content/styles/fonts/woff2_font.woff2");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/font-woff2");
            });
        }

        [Fact]
        public void read_EOT_file()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("content/styles/fonts/286692_0_0.eot");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/vnd.ms-fontobject");
            });
        }


        [Fact]
        public void read_SVG_file()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("content/styles/fonts/286692_0_0.svg");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("image/svg+xml");
            });
        }


        [Fact]
        public void read_TTF_file()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("content/styles/fonts/286692_0_0.ttf");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/octet-stream");
            });
        }
    }
}