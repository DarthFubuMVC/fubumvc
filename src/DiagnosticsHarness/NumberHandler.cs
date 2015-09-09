using System;
using FubuMVC.Core;

namespace DiagnosticsHarness
{
    public class NumberHandler
    {
        private readonly INumberCache _cache;

        public NumberHandler(INumberCache cache)
        {
            _cache = cache;
        }

        [WrapWith(typeof (DelayWrapper))]
        public void Consume(NumberMessage message)
        {
            if (message.Value > 100)
            {
                throw new ArithmeticException("Too big for me!");
            }

            _cache.Register(message.Value);
        }
    }
}