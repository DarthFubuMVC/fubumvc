using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Registration.DSL
{
    public class ViewsForActionFilterExpression
    {
        private readonly FubuRegistry _registry;

        public ViewsForActionFilterExpression(FubuRegistry registry)
        {
            _registry = registry;
        }

        /// <summary>
        /// views are matched to actions based on same namespace and the Action's underlying method name
        /// </summary>
        public void by_ViewModel_and_Namespace_and_MethodName()
        {
            @by<ActionWithSameNameAndFolderAsViewReturnsViewModelType>();
        }

        /// <summary>
        /// views are matched to Actions based on the view model (Action's output model -> view's ViewModel)
        /// and same namespace
        /// </summary>
        public void by_ViewModel_and_Namespace()
        {
            @by<ActionInSameFolderAsViewReturnsViewModelType>();
        }

        /// <summary>
        /// views are matched to Actions solely based on the view model (Action's output model -> view's ViewModel)
        /// </summary>
        public void by_ViewModel()
        {
            @by<ActionReturnsViewModelType>();
        }

        /// <summary>
        /// Specify your custom strategy to find attach views to Actions.
        /// </summary>
        public void @by<TFilter>() where TFilter : IViewsForActionFilter, new()
        {
            @by(new TFilter());
        }

        /// <summary>
        /// Specify your custom strategy to find attach views to Actions.
        /// </summary>
        public void @by(IViewsForActionFilter strategy)
        {
            _registry.AlterSettings<ViewAttachmentPolicy>(x => x.AddFilter(strategy));
        }
    }
}