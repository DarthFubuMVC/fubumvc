namespace FubuMVC.Diagnostics.Models
{
    public interface IModelBuilder<T>
        where T : class
    {
        T Build();
    }
}