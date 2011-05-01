using System;
using System.Linq;
using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View
{
    public class FubuMasterPage<TMasterPageViewModel> : FubuMasterPage, IFubuPage<TMasterPageViewModel>
        where TMasterPageViewModel : class
    {
        private TMasterPageViewModel _model;

        public TMasterPageViewModel Model
        {
            get
            {
                if (_model != null) return _model;

                var fubuPage = Page as IFubuPage;

                if (fubuPage != null)
                {
                    SetModel(fubuPage.ServiceLocator.GetInstance<IFubuRequest>());
                }

                return _model;
            }
        }

        public void SetModel(IFubuRequest request)
        {
            _model = request.Find<TMasterPageViewModel>().FirstOrDefault();
        }

        public void SetModel(object model)
        {
            _model = model as TMasterPageViewModel;
        }
    }

    public class FubuMasterPage : MasterPage, IFubuPage
    {
        public IFubuPage FubuPage { get { return (IFubuPage) Page; } }

        string IFubuPage.ElementPrefix{ get; set;}

        public IServiceLocator ServiceLocator
        {
            get { return FubuPage.ServiceLocator; }
            set { /* no-op */ }
        }

        public T Get<T>()
        {
            return FubuPage.Get<T>();
        }

        public T GetNew<T>()
        {
            return FubuPage.GetNew<T>();
        }

        public IUrlRegistry Urls
        {
            get { return FubuPage.Urls; }
        }
    }
}