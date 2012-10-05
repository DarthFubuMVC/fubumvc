namespace FubuMVC.Core.View.Model
{
    public interface IAttachRequest<T> where T : ITemplateFile
    {
        T Template { get; }
        ITemplateLogger Logger { get; }
    }

    public class AttachRequest<T> : IAttachRequest<T> where T : ITemplateFile
    {
        public T Template { get; set; }
        public ITemplateLogger Logger { get; set; }
    }
}