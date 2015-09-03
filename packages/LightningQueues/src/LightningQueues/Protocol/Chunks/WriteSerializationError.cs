using System.IO;
using System.Threading.Tasks;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public class WriteSerializationError : Chunk
    {
        public WriteSerializationError(ILogger logger) : base(logger)
        {
            
        }

        protected override async Task ProcessInternalAsync(Stream stream)
        {
            await stream.WriteAsync(ProtocolConstants.SerializationFailureBuffer, 0, ProtocolConstants.SerializationFailureBuffer.Length).ConfigureAwait(false);
        }
    }
}