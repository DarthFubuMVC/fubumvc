using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Registration.DSL
{
    public class ViewsForActionFilterExpression
    {
        private readonly ViewAttacherConvention _attacher;

        public ViewsForActionFilterExpression(ViewAttacherConvention attacher)
        {
            _attacher = attacher;
        }

        /// <summary>
        /// views are matched to actions based on same namespace and the Action's underlying method name
        /// </summary>
        public void by_ViewModel_and_Namespace_and_MethodName()
        {
            _attacher.AddViewsForActionFilter(new ActionWithSameNameAndFolderAsViewReturnsViewModelType());
        }

        /// <summary>
        /// views are matched to Actions based on the view model (Action's output model -> view's ViewModel)
        /// and same namespace
        /// </summary>
        public void by_ViewModel_and_Namespace()
        {
            _attacher.AddViewsForActionFilter(new ActionInSameFolderAsViewReturnsViewModelType());
        }

        /// <summary>
        /// views are matched to Actions solely based on the view model (Action's output model -> view's ViewModel)
        /// </summary>
        public void by_ViewModel()
        {
            _attacher.AddViewsForActionFilter(new ActionReturnsViewModelType());
        }

        /// <summary>
        /// Specify your custom strategy to find attach views to Actions.
        /// </summary>
        public void @by<TFilter>() where TFilter : IViewsForActionFilter, new()
        {
            _attacher.AddViewsForActionFilter(new TFilter());
        }

        /// <summary>
        /// Specify your custom strategy to find attach views to Actions.
        /// </summary>
        public void @by(IViewsForActionFilter strategy)
        {
            _attacher.AddViewsForActionFilter(strategy);
        }
    }
}