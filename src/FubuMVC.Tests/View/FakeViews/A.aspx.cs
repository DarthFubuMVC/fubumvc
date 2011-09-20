using System;
using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Tests.View.FakeViews
{
    public class A : Page, IFubuPage<ViewModel1>
    {

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

        public void Write(object content)
        {
            throw new NotImplementedException();
        }

        public object GetModel()
        {
            throw new NotImplementedException();
        }

        public IUrlRegistry Urls
        {
            get { throw new NotImplementedException(); }
        }

        public ViewModel1 Model { get; set; }
    }
}