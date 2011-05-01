using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Registration.DSL
{
    public class ViewsForActionFilterExpression
    {
        private readonly ViewAttacher _attacher;

        public ViewsForActionFilterExpression(ViewAttacher attacher)
        {
            _attacher = attacher;
        }

        public void by_ViewModel_and_Namespace_and_MethodName()
        {
            _attacher.AddViewsForActionFilter(new ActionWithSameNameAndFolderAsViewReturnsViewModelType());
        }

        public void by_ViewModel_and_Namespace()
        {
            _attacher.AddViewsForActionFilter(new ActionInSameFolderAsViewReturnsViewModelType());
        }

        public void by_ViewModel()
        {
            _attacher.AddViewsForActionFilter(new ActionReturnsViewModelType());
        }

        public void @by<TFilter>() where TFilter : IViewsForActionFilter, new()
        {
            _attacher.AddViewsForActionFilter(new TFilter());
        }

        public void @by(IViewsForActionFilter strategy)
        {
            _attacher.AddViewsForActionFilter(strategy);
        }
    }
}