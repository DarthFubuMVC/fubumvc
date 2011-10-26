namespace FubuMVC.Core.Resources.Etags
{
    public class ETaggedRequest
    {
        public string IfNoneMatch { get; set; }

        [ResourcePath]
        public string ResourcePath { get; set; }
    }
}