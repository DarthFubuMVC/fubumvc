using System;
using System.IO;
using System.Threading.Tasks;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public class WriteLength : Chunk
    {
        private readonly int _length;

        public WriteLength(ILogger logger, int length, string sender) : base(logger, sender)
        {
            _length = length;
        }

        public WriteLength(ILogger logger, int length) : this(logger, length, null)
        {
        }

        protected override Task ProcessInternalAsync(Stream stream)
        {
            var bufferLenInBytes = BitConverter.GetBytes(_length);
            return stream.WriteAsync(bufferLenInBytes, 0, bufferLenInBytes.Length);
        }

        public override string ToString()
        {
            return string.Format("Writing Length: {0}", _length);
        }
    }
}