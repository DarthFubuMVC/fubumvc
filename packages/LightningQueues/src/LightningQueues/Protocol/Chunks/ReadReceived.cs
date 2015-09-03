using System.IO;
using System.Text;
using System.Threading.Tasks;
using LightningQueues.Exceptions;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public class ReadReceived : Chunk
    {
        public ReadReceived(ILogger logger, string sender) : base(logger, sender)
        {
        }

        public ReadReceived(ILogger logger) : this(logger, null)
        {
        }

        protected override async Task ProcessInternalAsync(Stream stream)
        {
            var recieveBuffer = new byte[ProtocolConstants.RecievedBuffer.Length];
            await stream.ReadBytesAsync(recieveBuffer, "receive confirmation", false).ConfigureAwait(false);
            var recieveRespone = Encoding.Unicode.GetString(recieveBuffer);
            if (recieveRespone == ProtocolConstants.QueueDoesNotExists)
            {
                _logger.Info("Response from reciever {0} is that queue does not exists", _endpoint);
                throw new QueueDoesNotExistsException();
            }
            if (recieveRespone != ProtocolConstants.Recieved)
            {
                _logger.Info("Response from receiver {0} is not the expected one, unexpected response was: {1}",
                    _endpoint, recieveRespone);
                throw new UnexpectedReceivedMessageFormatException();
            }
        }
    }
}