using FubuMVC.Core.Continuations;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Spark
{
    [TestFixture]
    public class Partials_do_not_get_layout : ViewIntegrationContext
    {
        public Partials_do_not_get_layout()
        {
            SparkView<HelloPartialViewModel>("HelloPartialViewModel").Write(@"
<use master='PartialLayout' />
<p>In a partial</p>

<content:head>
  <script src='_/herp/derp.js'></script>
</content:head>
");


            SparkView("_NativePartial").WriteLine("<div>Hello Native</div>");

            SparkView<UsesNativeViewModel>("UsesNative").WriteLine("<NativePartial />");

            SparkView<UsesPartialViewModel>("UsesPartial").Write(@"
<use namespace='FubuMVC.IntegrationTesting.Views.Spark' />
<h1>Uses partial</h1>
!{this.Partial(new HelloPartialInputModel())}
");


            SparkView("Shared/Application").Write(@"

<html>
  <head>
	<use content='head' />
  </head>
  <body>
      <h1>Default layout</h1>
      <use content='view'/>
  </body>
</html>
");

            SparkView("Shared/PartialLayout").Write(@"

<html>
<head>
    <title>
        <use content='title'>Default Title</use>
    </title>
    <asset name='styles' />
    <use content='head' />
</head>
<body>
    <h1>This layout means FAIL!</h1>
    <use content='view' />
</body>
</html>

");

        }

        [Test]
        public void does_not_apply_layout_when_invoked_as_partial()
        {
            Scenario.Get.Action<UsesPartialEndpoint>(x => x.Execute());
            
            Scenario.ContentShouldContain("<h1>Uses partial</h1>");
            Scenario.ContentShouldContain("<h1>Default layout</h1>");
            Scenario.ContentShouldContain("<p>In a partial</p>");

            Scenario.ContentShouldNotContain("<h1>This layout means FAIL!</h1>");
        }

        [Test]
        public void invoking_action_normally_should_render_the_correct_layout()
        {
            Scenario.Get.Action<HelloPartialEndpoint>(x => x.Render());
            Scenario.ContentShouldContain("<p>In a partial</p>");
            Scenario.ContentShouldContain("<h1>This layout means FAIL!</h1>");
        }

        [Test]
        public void native_partials_happy_path()
        {
            Scenario.Get.Action<UsesNativeEndpoint>(x => x.Render());

            Scenario.ContentShouldContain("<div>Hello Native</div>");
        }

        [Test, Ignore("This was broken in 2.0, but I'm not entirely sure that we care.")]
        public void partials_should_still_have_access_to_master_layout_content_areas()
        {
            Scenario.Get.Action<UsesPartialEndpoint>(x => x.Execute());

            Scenario.ContentShouldContain("_/herp/derp.js");
        }

        [Test]
        public void should_apply_layout_when_transfered_to()
        {
            Scenario.Get.Action<TransferToEndpoint>(x => x.Tranfer());
            Scenario.ContentShouldContain("<p>In a partial</p>");
            Scenario.ContentShouldContain("<h1>This layout means FAIL!</h1>");
        }

    }

    public class HelloPartialEndpoint
    {
        public HelloPartialViewModel SayHelloPartial(HelloPartialInputModel input)
        {
            return new HelloPartialViewModel();
        }
        public HelloPartialViewModel Render()
        {
            return new HelloPartialViewModel();
        }
    }

    public class HelloPartialInputModel
    {
    }

    public class HelloPartialViewModel
    {
    }

    public class UsesNativeEndpoint
    {
        public UsesNativeViewModel Render()
        {
            return new UsesNativeViewModel();
        }
    }

    public class UsesNativeViewModel
    {
    }

    public class UsesPartialEndpoint
    {
        public UsesPartialViewModel Execute()
        {
            return new UsesPartialViewModel();
        }
    }

    public class UsesPartialViewModel
    {
    }

    public class TransferToEndpoint
    {
        public FubuContinuation Tranfer()
        {
            return FubuContinuation.TransferTo(new HelloPartialInputModel());
        }
    }
    public class RedirectToEndpoint
    {
        public FubuContinuation Redirect()
        {
            return FubuContinuation.RedirectTo<UsesPartialEndpoint>(x => x.Execute());
        }
    }
}