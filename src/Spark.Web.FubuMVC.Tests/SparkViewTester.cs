using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Spark.Web.FubuMVC.Tests
{
    [TestFixture]
    public class SparkViewTester
    {
        //[Test]
        //public void SiteRootActsAsSafePrefix()
        //{
        //    var mocks = new MockRepository();
        //    var httpContext = mocks.StrictMock<HttpContextBase>();
        //    var httpRequest = mocks.StrictMock<HttpRequestBase>();
        //    SetupResult.For(httpContext.Request).Return(httpRequest);

        //    var controller = mocks.StrictMock<ControllerBase>();

        //    Expect.Call(httpRequest.ApplicationPath).Return("/");
        //    Expect.Call(httpRequest.ApplicationPath).Return("/TestApp");
        //    Expect.Call(httpRequest.ApplicationPath).Return("/TestApp/");
        //    Expect.Call(httpRequest.ApplicationPath).Return("");
        //    Expect.Call(httpRequest.ApplicationPath).Return(null);
        //    Expect.Call(httpRequest.ApplicationPath).Return("TestApp/");
        //    Expect.Call(httpRequest.ApplicationPath).Return("TestApp");

        //    mocks.ReplayAll();

        //    var view = new StubSparkView();
        //    var viewContext = new ViewContext(new ControllerContext(httpContext, new RouteData(), controller), view, new ViewDataDictionary(), new TempDataDictionary());

        //    view = new StubSparkView { ViewContext = viewContext };
        //    Assert.AreEqual("", view.SiteRoot);

        //    view = new StubSparkView { ViewContext = viewContext };
        //    Assert.AreEqual("/TestApp", view.SiteRoot);

        //    view = new StubSparkView { ViewContext = viewContext };
        //    Assert.AreEqual("/TestApp", view.SiteRoot);

        //    view = new StubSparkView { ViewContext = viewContext };
        //    Assert.AreEqual("", view.SiteRoot);

        //    view = new StubSparkView { ViewContext = viewContext };
        //    Assert.AreEqual("", view.SiteRoot);

        //    view = new StubSparkView { ViewContext = viewContext };
        //    Assert.AreEqual("/TestApp", view.SiteRoot);

        //    view = new StubSparkView { ViewContext = viewContext };
        //    Assert.AreEqual("/TestApp", view.SiteRoot);

        //    mocks.VerifyAll();
        //}

        //[Test]
        //public void CanAccessModelViaModel()
        //{
        //    var view = new ModelViewTest { ViewData = { Model = "asd" } };
        //    Assert.AreEqual("asd", view.Model);
        //}

        //private class ModelViewTest : SparkView<string>
        //{
        //    public override void Render()
        //    {

        //        throw new NotImplementedException();

        //    }

        //    public override Guid GeneratedViewId
        //    {
        //        get { throw new NotImplementedException(); }
        //    }
        //}

        //private class StubSparkView : SparkView
        //{
        //    public override Guid GeneratedViewId
        //    {
        //        get { throw new NotImplementedException(); }
        //    }

        //    public override bool TryGetViewData(string name, out object value)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public override void Render()
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

    }
}
