using System;
using System.Net;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class RenderStatusCodeBehavior : IActionBehavior
    {
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;

        public RenderStatusCodeBehavior(IFubuRequest request, IOutputWriter writer)
        {
            _request = request;
            _writer = writer;
        }

        public void Invoke()
        {
            // There is a T Get<T>() method on IFubuRequest, but in
            // my infinite wisdom I made it so that it only works 
            // for reference types
            var status = (HttpStatusCode)_request.Get(typeof (HttpStatusCode));
            _writer.WriteResponseCode(status);
        }

        public void InvokePartial()
        {
            Invoke();
        }
    }
}