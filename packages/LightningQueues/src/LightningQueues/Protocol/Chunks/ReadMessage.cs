using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LightningQueues.Logging;
using LightningQueues.Model;

namespace LightningQueues.Protocol.Chunks
{
    public class ReadMessage : Chunk<Message[]>
    {
        private readonly int _length;

        public ReadMessage(ILogger logger, int length, string endpoint) : base(logger, endpoint)
        {
            _length = length;
        }

        public ReadMessage(ILogger logger, int length) : this(logger, length, null)
        {
        }

        protected async override Task<Message[]> GetInternalAsync(Stream stream)
        {
            var buffer = new byte[_length];
            await stream.ReadBytesAsync(buffer, "message data", false).ConfigureAwait(false);
            try
            {
                var messages = SerializationExtensions.ToMessages(buffer);
                _logger.Debug("Deserialized {0} messages from {1}", messages.Length, _endpoint);
                return messages;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to deserialize message", ex);
            }
        }

        public override string ToString()
        {
            return "Reading Message";
        }
    }
}