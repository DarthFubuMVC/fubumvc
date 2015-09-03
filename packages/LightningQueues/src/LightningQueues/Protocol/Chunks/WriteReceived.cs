using System.IO;
using System.Threading.Tasks;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public class WriteReceived : Chunk
    {
        public WriteReceived(ILogger logger, string endpoint) : base(logger, endpoint)
        {
        }

        public WriteReceived(ILogger logger) : this(logger, null)
        {
        }

        protected override Task ProcessInternalAsync(Stream stream)
        {
            _logger.Debug("Sending reciept notice to {0}", _endpoint);
            return stream.WriteAsync(ProtocolConstants.RecievedBuffer, 0, ProtocolConstants.RecievedBuffer.Length);
        }

        public override string ToString()
        {
            return "Write Received";
        }
    }
}