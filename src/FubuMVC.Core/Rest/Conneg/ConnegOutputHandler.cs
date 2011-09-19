using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Conneg
{
    public class ConnegOutputHandler<T> : IConnegOutputHandler where T : class
    {
        private readonly IMediaProcessor<T> _processor;

        public ConnegOutputHandler(IMediaProcessor<T> processor)
        {
            _processor = processor;
        }

        public void WriteOutput(CurrentRequest currentRequest, IFubuRequest request)
        {
            var output = request.Get<T>();
            _processor.Write(output, currentRequest);
        }
    }
}