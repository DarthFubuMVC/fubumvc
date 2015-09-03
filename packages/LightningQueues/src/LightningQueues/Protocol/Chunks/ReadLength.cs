using System;
using System.IO;
using System.Threading.Tasks;
using LightningQueues.Exceptions;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public class ReadLength : Chunk<int>
    {
        public ReadLength(ILogger logger, string sender) : base(logger, sender)
        {
        }

        public ReadLength(ILogger logger) : this(logger, null)
        {
        }

        protected override async Task<int> GetInternalAsync(Stream stream)
        {
            var lenOfDataToReadBuffer = new byte[sizeof(int)];
            await stream.ReadBytesAsync(lenOfDataToReadBuffer, "length data", false).ConfigureAwait(false);

            var lengthOfDataToRead = BitConverter.ToInt32(lenOfDataToReadBuffer, 0);
            if (lengthOfDataToRead < 0)
            {
                throw new InvalidLengthException(string.Format("Got invalid length {0} from endpoint {1}",
                                                               lengthOfDataToRead, _endpoint));
            }
            return lengthOfDataToRead;
        }

        public override string ToString()
        {
            return "Reading Length ";
        }
    }
}