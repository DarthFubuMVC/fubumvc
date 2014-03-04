using FubuMVC.Razor.Rendering;

namespace FubuMVC.Razor.Tests.Rendering
{
    public class StubView : FubuRazorView<PersonViewModel>
    {
        public override void Execute()
        {
        }
    }
}