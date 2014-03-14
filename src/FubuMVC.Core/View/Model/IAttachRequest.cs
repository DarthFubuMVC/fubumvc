using FubuCore;

namespace FubuMVC.Core.View.Model
{
    [MarkedForTermination]
    public interface IAttachRequest<T> where T : ITemplateFile
    {
        T Template { get; }
        ITemplateLogger Logger { get; }
    }

    [MarkedForTermination]
    public class AttachRequest<T> : IAttachRequest<T> where T : ITemplateFile
    {
        public T Template { get; set; }
        public ITemplateLogger Logger { get; set; }
    }
}