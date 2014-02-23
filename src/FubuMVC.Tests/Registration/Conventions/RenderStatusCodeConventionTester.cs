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
    public class InputMessage 
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