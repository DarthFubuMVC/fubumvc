using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View.Model
{
    public interface IBindRequest<T> where T : ITemplateFile
    {
        T Target { get; }
        Parsing Parsing { get; }

        TypePool Types { get; }
        ITemplateRegistry<T> TemplateRegistry { get; }
        ITemplateLogger Logger { get; }
    }

    public class BindRequest<T> : IBindRequest<T> where T : ITemplateFile
    {
        public T Target { get; set; }
        public Parsing Parsing { get; set; }
        public TypePool Types { get; set; }

        public ITemplateRegistry<T> TemplateRegistry { get; set; }
        public ITemplateLogger Logger { get; set; }
    }

    public interface ITemplateBinder<T> where T : ITemplateFile
    {
        bool CanBind(IBindRequest<T> request);
        void Bind(IBindRequest<T> request);
    }
}