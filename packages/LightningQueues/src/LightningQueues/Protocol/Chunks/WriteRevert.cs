using System.IO;
using System.Threading.Tasks;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public class WriteRevert : Chunk
    {
        public WriteRevert(ILogger logger, string endpoint) : base(logger, endpoint)
        {
        }

        public WriteRevert(ILogger logger) : this(logger, null)
        {
        }

        protected override Task ProcessInternalAsync(Stream stream)
        {
            return stream.WriteAsync(ProtocolConstants.RevertBuffer, 0, ProtocolConstants.RevertBuffer.Length);
        }

        public override string ToString()
        {
            return "Write Revert";
        }
    }
}