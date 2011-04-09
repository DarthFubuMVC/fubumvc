using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class DeserializeJsonBehavior<T> : BasicBehavior where T : class
    {
        private readonly IJsonReader _reader;
        private readonly IFubuRequest _request;

        public DeserializeJsonBehavior(IJsonReader reader, IFubuRequest request) : base(PartialBehavior.Executes)
        {
            _reader = reader;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            var input = _reader.Read<T>();
            _request.Set(input);

            return DoNext.Continue;
        }
    }
}