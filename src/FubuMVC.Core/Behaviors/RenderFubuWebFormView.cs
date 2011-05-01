using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.View.WebForms;

namespace FubuMVC.Core.Behaviors
{

    public class RenderFubuWebFormView : RenderFubuViewBehavior
    {
        public RenderFubuWebFormView(WebFormViewEngine<IFubuPage> engine, IFubuRequest request, ViewPath view, IPageActivator activator)
            : base(engine, request, view, activator)
        {
        }
    }

    public class RenderFubuViewBehavior : BasicBehavior
    {
        private readonly IViewEngine<IFubuPage> _engine;
        private readonly IFubuRequest _request;
        private readonly ViewPath _path;
        private readonly IPageActivator _activator;

        public RenderFubuViewBehavior(IViewEngine<IFubuPage> engine, IFubuRequest request, ViewPath view, IPageActivator activator)
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
                var withModel = v as IFubuPageWithModel;

                if (withModel != null)
                {
                    withModel.SetModel(_request);
                }

                ActivateView(_activator, v);
            });

            return DoNext.Continue;
        }

        public static void ActivateView(IPageActivator activator, IFubuPage view)
        {
            var closedInterface = view.GetType().FindInterfaceThatCloses(typeof(IFubuPage<>));
            if (closedInterface != null)
            {
                var parameterType = closedInterface.GetGenericArguments().First();
                var activatorType = typeof(TemplatedActivator<>).MakeGenericType(parameterType);
                var specificActivator = (IActivator)Activator.CreateInstance(activatorType);

                specificActivator.Activate(activator, view);
            }

            var page = view as IFubuPage;
            if (page == null) return;

            activator.Activate(page);
        }

        public interface IActivator
        {
            void Activate(IPageActivator activator, IFubuPage view);
        }

        public class TemplatedActivator<T> : IActivator where T : class
        {
            public void Activate(IPageActivator activator, IFubuPage view)
            {
                activator.Activate((IFubuPage<T>)view);
            }
        }
    }







}