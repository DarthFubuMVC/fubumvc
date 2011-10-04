using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Media.Atom
{
    public class DirectFeedSource<TModel, T> : IFeedSource<T> where TModel : class, IEnumerable<IValues<T>>
    {
        private readonly IFubuRequest _request;

        public DirectFeedSource(IFubuRequest request)
        {
            _request = request;
        }

        public IEnumerable<IValues<T>> GetValues()
        {
            return _request.Get<TModel>();
        }
    }
}