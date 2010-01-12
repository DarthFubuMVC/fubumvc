using FubuMVC.Core.View;

namespace FubuMVC.Core.Registration.DSL
{
    public class ViewAttachmentStrategyExpression
    {
        private readonly ViewAttacher _attacher;

        public ViewAttachmentStrategyExpression(ViewAttacher attacher)
        {
            _attacher = attacher;
        }

        public void by_ViewModel_and_Namespace_and_MethodName()
        {
            _attacher.AddAttachmentStrategy(new TypeAndNamespaceAndName());
        }

        public void by_ViewModel_and_Namespace()
        {
            _attacher.AddAttachmentStrategy(new TypeAndNamespace());
        }

        public void by_ViewModel()
        {
            _attacher.AddAttachmentStrategy(new UniqueTypeMatcher());
        }

        public void @by<T>() where T : IViewAttachmentStrategy, new()
        {
            _attacher.AddAttachmentStrategy(new T());
        }

        public void @by(IViewAttachmentStrategy strategy)
        {
            _attacher.AddAttachmentStrategy(strategy);
        }
    }
}