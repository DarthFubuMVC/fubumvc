using System;
using System.Web.UI;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

namespace FubuMVC.Tests.View.FakeViews.Folder1
{
    public class ViewModel3 : Page, IFubuView<ViewModel3>
    {
        public void SetModel(IFubuRequest request)
        {
            throw new NotImplementedException();
        }

        public ViewModel3 Model { get { throw new NotImplementedException(); } }
    }
}