using System;
using FubuCore;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime
{
    public class FubuSmartRequest : SmartRequest
    {
        private readonly IRequestData _data;
        private readonly IObjectConverter _converter;
        private readonly IFubuRequest _request;

        public FubuSmartRequest(IRequestData data, IObjectConverter converter, IFubuRequest request) : base(data, converter)
        {
            _data = data;
            _converter = converter;
            _request = request;
        }

        public override object Value(Type type, string key)
        {
            var value = base.Value(type, key);
            if (value == null)
            {
                value = _request.Get(type);
            }

            return value;
        }

    }
}