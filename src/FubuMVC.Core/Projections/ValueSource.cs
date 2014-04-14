using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Projections
{
    public class ValueSource<T> : IValueSource<T> where T : class
    {
        private readonly IFubuRequest _request;

        public ValueSource(IFubuRequest request)
        {
            _request = request;
        }

        public IValues<T> FindValues()
        {
            var target = _request.Get<T>();
            return new SimpleValues<T>(target);
        }
    }
}