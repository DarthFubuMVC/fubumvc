using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Media.Atom
{
    public class EnumerableFeedSource<TModel, T> : IFeedSource<T> where TModel : class, IEnumerable<T>
    {
        private readonly IFubuRequest _request;

        public EnumerableFeedSource(IFubuRequest request)
        {
            _request = request;
        }

        public IEnumerable<IValues<T>> GetValues()
        {
            return _request.Get<TModel>().Select(x => new SimpleValues<T>(x));
        }
    }
}