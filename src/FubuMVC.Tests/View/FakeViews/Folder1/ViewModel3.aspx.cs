using System;
using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Tests.View.FakeViews.Folder1
{
    public class ViewModel3 : Page, IFubuPage<ViewModel3>
    {
        public void SetModel(IFubuRequest request)
        {
            throw new NotImplementedException();
        }

        public void SetModel(object model)
        {
            throw new NotImplementedException();
        }

        public ViewModel3 Model { get; set; }

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
    }
}