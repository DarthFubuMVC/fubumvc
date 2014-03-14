using FubuCore;

namespace FubuMVC.Core.View.Model
{
    [MarkedForTermination]
    public interface ISharingAttacher<T> where T : ITemplateFile
    {
        bool CanAttach(IAttachRequest<T> request);
        void Attach(IAttachRequest<T> request);
    }
}