using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.UI
{
    public class PartialInvoker : IPartialInvoker
    {
        private readonly IPartialFactory _factory;
        private readonly IFubuRequest _request;
        private readonly IAuthorizationPreviewService _authorization;
        private readonly ITypeResolver _types;

        public PartialInvoker(IPartialFactory factory, IFubuRequest request, IAuthorizationPreviewService authorization, ITypeResolver types)
        {
            _factory = factory;
            _request = request;
            _authorization = authorization;
            _types = types;
        }

        public void Invoke<T>() where T : class
        {
            var input = _request.Get<T>();
            if (_authorization.IsAuthorized(input))
            {
                _factory.BuildPartial(typeof(T)).InvokePartial();
            }
        }

        public void InvokeObject(object model)
        {
            if (_authorization.IsAuthorized(model))
            {
                var requestType = _types.ResolveType(model);
                _request.Set(requestType, model);
                _factory.BuildPartial(requestType).InvokePartial();
            }
        }
    }
}