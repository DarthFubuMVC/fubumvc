using System.Collections.Generic;
using FubuMVC.Core.View.Activation;

namespace FubuMVC.Core.View.Rendering
{
    public interface IViewModifier<T> where T : IRenderableView
    {
        bool Applies(T view);
        T Modify(T view);
    }

    public interface IViewModifierService<T> where T : IRenderableView
    {
        T Modify(T view);
    }

    public class ViewModifierService<T> : IViewModifierService<T> where T : IRenderableView
    {
        private readonly IEnumerable<IViewModifier<T>> _modifications;

        public ViewModifierService(IEnumerable<IViewModifier<T>> modifications)
        {
            _modifications = modifications;
        }

        public T Modify(T view)
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

    public abstract class BasicViewModifier<T> : IViewModifier<T> where T : IRenderableView
    {
        public virtual bool Applies(T view) { return true; }
        public abstract T Modify(T view);
    }

    public class PageActivation<T> : BasicViewModifier<T> where T : IRenderableView
    {
        private readonly IPageActivator _activator;
        public PageActivation(IPageActivator activator)
        {
            _activator = activator;
        }

        public override T Modify(T view)
        {
            return view.Modify(v => _activator.Activate(v));
        }
    }
}