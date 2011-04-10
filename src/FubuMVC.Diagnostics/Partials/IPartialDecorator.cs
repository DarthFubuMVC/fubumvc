namespace FubuMVC.Diagnostics.Partials
{
    public interface IPartialDecorator<T>
        where T : class
    {
        T Enrich(T target);
    }
}