using System;
using System.Net;
using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class RenderStatusCodeConventionTester
    {
        [Test, Ignore("Just for now")]
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
    public class InputMessage : JsonMessage
    {
    }

    public class RestfulService
    {
        public HttpStatusCode Action1(InputMessage message)
        {
            // perform the action
            return HttpStatusCode.OK;
        }
    }
}