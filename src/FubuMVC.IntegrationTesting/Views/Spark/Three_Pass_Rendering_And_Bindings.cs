using System.Text;
using FubuCore.Descriptions;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class Three_Pass_Rendering_And_Bindings : ViewIntegrationContext
    {
        public Three_Pass_Rendering_And_Bindings()
        {
            SparkView("Shared/_Sidebar").Write(@"
<content name='head'>
<script src='~/content/js/jquery-ui-accordian.js' once='jquery-accordian'></script>
</content>
<div id='sidebar'>Say hello: <SayHello /></div>
");

            SparkView("Shared/Application").Write(@"
<use master='Html'/>
<use content='view'/>
<Sidebar />
");

            File("Shared/bindings.xml").Write(@"

<bindings>
  <element name='SayHello'>'Hello'</element>
</bindings>

");

            SparkView("Shared/Html").Write(@"

<html>
<head>
<use content='head'/>
</head>
<body>
<use content='view'/>
<use content='tail'/>
</body>
</html>
");

            SparkView<ThreePassModel>("ThreePassSample").Write("<h1>${Model.Message}</h1>");
        }

//
//        [SetUp]
//        public void SetUp()
//        {
//            theResult = new StringBuilder()
//                .AppendLine("<html>")
//                .AppendLine("<head>")
//                .AppendLine(@"<script src=""/content/js/jquery-ui-accordian.js""></script>")
//                .AppendLine("</head>")
//                .AppendLine("<body>")
//                .AppendLine("<h1>Three Pass Rendering Test!</h1>")
//                .AppendLine(@"<div id=""sidebar"">Say hello: Hello</div>")
//                .AppendLine("</body>")
//                .Append("</html>").Replace("\r", string.Empty)
//                .ToString();
//        }

        [Test, Explicit("Just too flakey.  Blaming the file system")]
        public void three_pass_renders_correctly()
        {
            

            Scenario.Get.Action<ThreePassEndpoint>(x => x.ThreePassSample(null));

            Scenario.ContentShouldContain("<html>");
            Scenario.ContentShouldContain("<head>");
            Scenario.ContentShouldContain(@"<script src=""/content/js/jquery-ui-accordian.js""></script>");
            Scenario.ContentShouldContain("</head>");
            Scenario.ContentShouldContain("<body>");
            Scenario.ContentShouldContain("<h1>Three Pass Rendering Test!</h1>");
            Scenario.ContentShouldContain(@"<div id=""sidebar"">Say hello: Hello</div>");
            Scenario.ContentShouldContain("</body>");
        }
    }


    public class ThreePassEndpoint
    {
        public ThreePassModel ThreePassSample(ThreePassModel input)
        {
            return input;
        }
    }

    public class ThreePassModel
    {
        public ThreePassModel()
        {
            Message = "Three Pass Rendering Test!";
        }
        public string Message { get; set; }
    }
}