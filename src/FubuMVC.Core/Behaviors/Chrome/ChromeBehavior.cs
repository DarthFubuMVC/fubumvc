using System;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;

namespace FubuMVC.Core.Behaviors.Chrome
{
    // Look in integration testing for this one.
    public class ChromeBehavior<T> : IActionBehavior where T : ChromeContent, new()
    {
        private readonly IPartialInvoker _partials;
        private readonly IActionBehavior _inner;
        private readonly IOutputWriter _writer;
        private readonly Action<ChromeContent> _customization;

        public ChromeBehavior(IPartialInvoker partials, IActionBehavior inner, IOutputWriter writer, Action<ChromeContent> customization)
        {
            _partials = partials;
            _inner = inner;
            _writer = writer;
            _customization = customization;
        }

        public void Invoke()
        {
            var output = _writer.Record(() => _inner.Invoke());

            var subject = new T{
                InnerContent = output.GetText()
            };

            _customization(subject);

            var html = _partials.InvokeObject(subject);
        
            _writer.WriteHtml(html);
        }

        public void InvokePartial()
        {
            _inner.InvokePartial();
        }
    }
}