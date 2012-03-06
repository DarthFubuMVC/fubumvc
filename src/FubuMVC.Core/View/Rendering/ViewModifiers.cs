using System.Collections.Generic;
using FubuMVC.Core.View.Activation;

namespace FubuMVC.Core.View.Rendering
{
    public interface IViewModifier
    {
        bool Applies(IRenderableView view);
        IRenderableView Modify(IRenderableView view);
    }

    public interface IViewModifierService
    {
        IRenderableView Modify(IRenderableView view);
    }

    public class ViewModifierService : IViewModifierService
    {
        private readonly IEnumerable<IViewModifier> _modifications;

        public ViewModifierService(IEnumerable<IViewModifier> modifications)
        {
            _modifications = modifications;
        }

        public IRenderableView Modify(IRenderableView view)
        {
            foreach (var modification in _modifications)
            {
                if (modification.Applies(view))
                {
                    view = modification.Modify(view); // consider if we should add a "?? view;" or just let it fail
                }
            }
            return view;
        }
    }

    public abstract class BasicViewModifier : IViewModifier
    {
        public virtual bool Applies(IRenderableView view) { return true; }
        public abstract IRenderableView Modify(IRenderableView view);
    }

    public class PageActivation : BasicViewModifier
    {
        private readonly IPageActivator _activator;
        public PageActivation(IPageActivator activator)
        {
            _activator = activator;
        }

        public override IRenderableView Modify(IRenderableView view)
        {
            return view.Modify(v => _activator.Activate(v));
        }
    }
}