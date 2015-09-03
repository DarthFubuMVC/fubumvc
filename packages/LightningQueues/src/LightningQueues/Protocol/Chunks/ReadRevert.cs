using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LightningQueues.Exceptions;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public class ReadRevert : Chunk
    {
        public ReadRevert(ILogger logger, string sender) : base(logger, sender)
        {
        }

        public ReadRevert(ILogger logger) : this(logger, null)
        {
        }

        protected override async Task ProcessInternalAsync(Stream stream)
        {
            var buffer = new byte[ProtocolConstants.RevertBuffer.Length];
            try
            {
                await stream.ReadBytesAsync(buffer, "revert", true).ConfigureAwait(false);
            }
            catch (Exception)
            {
                //a revert was not sent, send is complete
                return;
            }
            var revert = Encoding.Unicode.GetString(buffer);
            if (revert == ProtocolConstants.Revert)
            {
                throw new RevertSendException();
            }
        }
    }
}