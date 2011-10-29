namespace FubuMVC.Core.Resources.Etags
{
    public class ETagTuple<T>
    {
        public T Target { get; set; }
        public ETaggedRequest Request { get; set; }
    }
}