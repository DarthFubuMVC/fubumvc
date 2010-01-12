using System;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core;

namespace FubuMVC.Core.View
{
    public class RenderFubuViewBehavior : BasicBehavior
    {
        private readonly IViewEngine<IFubuView> _engine;
        private readonly IFubuRequest _request;
        private readonly ViewPath _path;
        private readonly IViewActivator _activator;

        public RenderFubuViewBehavior(IViewEngine<IFubuView> engine, IFubuRequest request, ViewPath view, IViewActivator activator)
            : base(PartialBehavior.Executes)
        {
            _engine = engine;
            _request = request;
            _path = view;
            _activator = activator;
        }

        public ViewPath View { get { return _path; } }

        protected override DoNext performInvoke()
        {
            _engine.RenderView(_path, v =>
            {
                var withModel = v as IFubuViewWithModel;

                if (withModel != null)
                {
                    withModel.SetModel(_request);
                }

                ActivateView(_activator, v);
            });

            return DoNext.Continue;
        }

        public static void ActivateView(IViewActivator activator, IFubuView view )
        {
            var closedInterface = view.GetType().FindInterfaceThatCloses(typeof (IFubuPage<>));
            if (closedInterface != null)
            {
                var parameterType = closedInterface.GetGenericArguments().First();
                var activatorType = typeof (TemplatedActivator<>).MakeGenericType(parameterType);
                var specificActivator = (IActivator)Activator.CreateInstance(activatorType);
            
                specificActivator.Activate(activator, view);
            }

            var page = view as IFubuPage;
            if (page == null) return;

            activator.Activate(page);
        }

        public interface IActivator
        {
            void Activate(IViewActivator activator, IFubuView view);
        }

        public class TemplatedActivator<T> : IActivator where T : class
        {
            public void Activate(IViewActivator activator, IFubuView view)
            {
                activator.Activate((IFubuPage<T>)view);
            }
        }
    }

    public interface IViewActivator
    {
        void Activate<T>(IFubuPage<T> page) where T : class;
        void Activate(IFubuPage page);
    }

    public class NulloViewActivator : IViewActivator
    {
        public void Activate<T>(IFubuPage<T> page) where T : class
        {
            
        }

        public void Activate(IFubuPage page)
        {
            
        }
    }
}