using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;

namespace FubuMVC.Diagnostics.Chrome
{
    public class ChromeBehavior : IActionBehavior
    {
        private readonly IPartialInvoker _partials;
        private readonly IActionBehavior _inner;
        private readonly IOutputWriter _writer;

        public ChromeBehavior(IPartialInvoker partials, IActionBehavior inner, IOutputWriter writer)
        {
            _partials = partials;
            _inner = inner;
            _writer = writer;
        }

        public void Invoke()
        {
            var output = _writer.Record(() => _inner.Invoke());

            var subject = new ChromeContent{
                InnerContent = output.GetText()
            };

            var html = _partials.InvokeObject(subject);
        
            _writer.WriteHtml(html);
        }

        public void InvokePartial()
        {
            _inner.InvokePartial();
        }
    }
}