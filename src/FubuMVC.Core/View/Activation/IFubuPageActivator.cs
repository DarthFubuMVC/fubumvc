using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuCore;
using FubuMVC.Core.Runtime;
using HtmlTags.Conventions;

namespace FubuMVC.Core.View.Activation
{
    public interface IFubuPageActivator
    {
        void Activate(IFubuPage page);
        void Deactivate(IFubuPage page);
    }

    public class FubuPageActivator<T> : IFubuPageActivator where T : class
    {
        private readonly IServiceLocator _services;
        private readonly IViewToken _view;

        public FubuPageActivator(IServiceLocator services, IViewToken view)
        {
            _services = services;
            _view = view;
        }

        public void Activate(IFubuPage page)
        {
            page.ServiceLocator = _services;
            page.As<IFubuPage<T>>().Model = findModel();

            if (_view.ProfileName.IsNotEmpty())
            {
                page.Get<ActiveProfile>().Push(_view.ProfileName);
            }
        }

        private T findModel()
        {
            //We need to see if there is an exact match first.
            var request = _services.GetInstance<IFubuRequest>();
            if (request.Has<T>()) return request.Get<T>();

            // No exact match, so try to find something that *could*
            // be cast to T
            var model = request.Find<T>().FirstOrDefault();
            if (model != null) return model;

            // Force the model binding to find T
            return request.Get<T>();
        }

        public void Deactivate(IFubuPage page)
        {
            if (_view.ProfileName.IsNotEmpty())
            {
                page.Get<ActiveProfile>().Pop();
            }
        }
    }

    

}
