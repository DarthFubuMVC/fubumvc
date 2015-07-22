using FubuMVC.Razor.Rendering;

namespace FubuMVC.IntegrationTesting.Views.Razor.Rendering
{
    public class StubView : FubuRazorView<PersonViewModel>
    {
        public override void Execute()
        {
        }
    }
}