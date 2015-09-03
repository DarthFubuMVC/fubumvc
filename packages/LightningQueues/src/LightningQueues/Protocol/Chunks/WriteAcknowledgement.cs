using System.IO;
using System.Threading.Tasks;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public class WriteAcknowledgement : Chunk
    {
        public WriteAcknowledgement(ILogger logger, string sender) : base(logger, sender)
        {
        }

        public WriteAcknowledgement(ILogger logger) : this(logger, null)
        {
        }

        protected override Task ProcessInternalAsync(Stream stream)
        {
            return stream.WriteAsync(ProtocolConstants.AcknowledgedBuffer, 0, ProtocolConstants.AcknowledgedBuffer.Length);
        }

        public override string ToString()
        {
            return string.Format("Writing Acknowledgement");
        }
    }
}