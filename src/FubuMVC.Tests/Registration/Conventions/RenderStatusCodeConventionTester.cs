using System;
using System.Diagnostics;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Tests.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;
using RestfulStatusCodeServices;
using System.Linq;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class RenderStatusCodeConventionTester
    {
        [Test]
        public void just_redo_all_of_this()
        {
            throw new NotImplementedException();
        }
    }
}

namespace RestfulStatusCodeServices
{
    // JsonMessage is just a marker interface
    // FubuMVC doesn't yet support "conneg" -- but should
    // We take pull requests!
    public class InputMessage : JsonMessage {}

    public class RestfulService
    {
        public HttpStatusCode Action1(InputMessage message)
        {
            // perform the action
            return HttpStatusCode.OK;
        }
    }

}