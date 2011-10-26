namespace FubuMVC.Core.Resources.Etags
{
    public interface IETagGenerator<T>
    {
        string Create(T target);
    }
}