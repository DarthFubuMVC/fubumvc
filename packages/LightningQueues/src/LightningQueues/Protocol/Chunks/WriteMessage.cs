using System.IO;
using System.Threading.Tasks;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public class WriteMessage : Chunk
    {
        private readonly byte[] _buffer;

        public WriteMessage(ILogger logger, byte[] buffer, string sender) : base(logger, sender)
        {
            _buffer = buffer;
        }

        public WriteMessage(ILogger logger, byte[] buffer) : this(logger, buffer, null)
        {
        }

        protected override Task ProcessInternalAsync(Stream stream)
        {
            return stream.WriteAsync(_buffer, 0, _buffer.Length);
        }

        public override string ToString()
        {
            return string.Format("Writing Bytes {0}", _buffer.Length);
        }
    }
}