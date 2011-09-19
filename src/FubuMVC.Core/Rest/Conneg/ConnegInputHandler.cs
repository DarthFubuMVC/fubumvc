using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Conneg
{
    public class ConnegInputHandler<T> : IConnegInputHandler where T : class
    {
        private readonly IMediaProcessor<T> _processor;

        public ConnegInputHandler(IMediaProcessor<T> processor)
        {
            _processor = processor;
        }

        public void ReadInput(CurrentRequest currentRequest, IFubuRequest request)
        {
            var input = _processor.Retrieve(currentRequest);
            request.Set(input);
        }
    }
}