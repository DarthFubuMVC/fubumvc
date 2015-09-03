using System.IO;
using System.Threading.Tasks;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public class WriteProcessingError : Chunk
    {
        private readonly byte[] _errorBytes;

        public WriteProcessingError(ILogger logger, byte[] errorBytes, string endpoint) : base(logger, endpoint)
        {
            _errorBytes = errorBytes;
        }

        public WriteProcessingError(ILogger logger, byte[] errorBytes) : this(logger, errorBytes, null)
        {
        }

        protected override Task ProcessInternalAsync(Stream stream)
        {
            return stream.WriteAsync(_errorBytes, 0, _errorBytes.Length);
        }

        public override string ToString()
        {
            return string.Format("Write Processing Error: {0}", System.Text.Encoding.Unicode.GetString(_errorBytes));
        }
    }
}