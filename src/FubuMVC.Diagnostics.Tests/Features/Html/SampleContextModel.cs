namespace FubuMVC.Diagnostics.Tests.Features.Html
{
    public class SampleContextModel
    {
        public IUnusableModel UnusableLink { get; set; }
        public SampleContextModel Child { get; set; }
        public string Public { get; set; }
        private string Private { get; set; }
    }

    public interface IUnusableModel { }
}