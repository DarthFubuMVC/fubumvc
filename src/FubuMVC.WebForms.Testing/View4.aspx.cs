using System;
using System.Web.UI;
using FubuCore;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;

namespace FubuMVC.WebForms.Testing
{
    public class View4 : Page, IFubuPage<ViewModel4>
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
        public ViewModel4 Model { get; set; }
    }


}