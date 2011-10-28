namespace FubuMVC.Core.Resources.Etags
{
    public class ETaggedRequest
    {
        public string IfNoneMatch { get; set; }

        [ResourceHash]
        public string ResourceHash { get; set; }
    }
}