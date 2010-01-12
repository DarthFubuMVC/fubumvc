using System;
using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

namespace FubuMVC.Tests.View.FakeViews.Folder1
{
    public class A : Page, IFubuView<ViewModel1>
    {
        public void SetModel(IFubuRequest request)
        {
            throw new NotImplementedException();
        }

        public ViewModel1 Model { get { throw new NotImplementedException(); } }
    }
}