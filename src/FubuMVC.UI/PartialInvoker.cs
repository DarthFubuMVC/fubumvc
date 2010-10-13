using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

namespace FubuMVC.UI
{
    public class PartialInvoker : IPartialInvoker
    {
        private readonly IPartialFactory _factory;
        private readonly IFubuRequest _request;
        private readonly IAuthorizationPreviewService _authorization;

        public PartialInvoker(IPartialFactory factory, IFubuRequest request, IAuthorizationPreviewService authorization)
        {
            _factory = factory;
            _request = request;
            _authorization = authorization;
        }

        public void Invoke<T>() where T : class
        {
            var input = _request.Get<T>();
            if (_authorization.IsAuthorized(input))
            {
                _factory.BuildPartial(typeof(T)).InvokePartial();
            }
        }
    }
}