namespace FubuMVC.Diagnostics.Partials
{
    public interface IPartialDecorator<T>
        where T : class, IPartialModel
    {
        T Enrich(T target);
    }
}