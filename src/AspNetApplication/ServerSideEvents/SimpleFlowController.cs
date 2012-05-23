using System.Threading.Tasks;

namespace AspNetApplication.ServerSideEvents
{
    public class SimpleFlowController
    {
        private readonly EventSourceWriter<SimpleInput> _writer;

        public SimpleFlowController(EventSourceWriter<SimpleInput> writer)
        {
            _writer = writer;
        }

        public Task get_events_simple(SimpleInput input)
        {
            return _writer.Write(input);
        }
    }
}