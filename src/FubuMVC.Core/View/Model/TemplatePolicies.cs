namespace FubuMVC.Core.View.Model
{
    public interface ITemplatePolicy<T> where T : ITemplateFile
    {
        bool Matches(T template);
        void Apply(T template);
    }

}