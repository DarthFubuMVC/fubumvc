using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace FubuMVC.WebForms
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
                
                var fubuPage = Page as IFubuPageWithModel;

                if (fubuPage != null)
                {
                    Model = fubuPage.GetModel() as TMasterPageViewModel;
                }

                if (_model == null)
                {
                    _model = Get<IFubuRequest>().Find<TMasterPageViewModel>().FirstOrDefault();
                }

                return _model;
            }
            set
            {
                _model = value;
            }
        }

        public object GetModel()
        {
            return Model;
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

        public void Write(object content)
        {
            FubuPage.Write(content);
        }

        public IUrlRegistry Urls
        {
            get { return FubuPage.Urls; }
        }
    }
}