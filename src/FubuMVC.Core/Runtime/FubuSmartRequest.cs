using System;
using FubuCore.Binding;
using FubuCore.Conversion;

namespace FubuMVC.Core.Runtime
{
    public class FubuSmartRequest : SmartRequest
    {
        private readonly IFubuRequest _request;

        public FubuSmartRequest(IRequestData data, IObjectConverter converter, IFubuRequest request)
            : base(data, converter)
        {
            _request = request;
        }

        public override object Value(Type type, string key)
        {
            return base.Value(type, key) ?? _request.Get(type);
        }
    }
}