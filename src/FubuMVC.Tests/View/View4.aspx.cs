using System;
using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Tests.View.FakeViews;

namespace FubuMVC.Tests.View
{
    public class View4 : Page, IFubuView<ViewModel4>
    {
        public void SetModel(IFubuRequest request)
        {
            throw new NotImplementedException();
        }

        public void SetModel(object model)
        {
            throw new NotImplementedException();
        }

        public ViewModel4 Model { get { throw new NotImplementedException(); } }
    }
}