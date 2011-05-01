using System;
using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Tests.View.FakeViews;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Tests.View
{
    public class View4 : Page, IFubuPage<ViewModel4>
    {
        public void SetModel(IFubuRequest request)
        {
            throw new NotImplementedException();
        }

        public string ElementPrefix
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IServiceLocator ServiceLocator
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public T Get<T>()
        {
            throw new NotImplementedException();
        }

        public T GetNew<T>()
        {
            throw new NotImplementedException();
        }

        public IUrlRegistry Urls
        {
            get { throw new NotImplementedException(); }
        }

        public void SetModel(object model)
        {
            throw new NotImplementedException();
        }

        public ViewModel4 Model { get { throw new NotImplementedException(); } }
    }
}