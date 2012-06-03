using FubuCore;

namespace FubuMVC.Diagnostics.Navigation
{
    [MarkedForTermination]
    public class NavigationMenuItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
    }
}