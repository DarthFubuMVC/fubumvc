using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LightningQueues.Exceptions;
using LightningQueues.Logging;
using LightningQueues.Model;
using LightningQueues.Protocol.Chunks;

namespace LightningQueues.Protocol
{
    public class ReceivingProtocol
    {
        // Public for testing
        public static ILogger Logger = LogManager.GetLogger<ReceivingProtocol>();

        public async Task ReadMessagesAsync(string endpoint, Stream stream,
            Func<Message[], IMessageAcceptance> acceptMessages)
        {
            try
            {
                await readMessageAsync(endpoint, stream, acceptMessages).ConfigureAwait(false);
            }
            catch (ObjectDisposedException)
            {
                // Swallowing this so we don't have unobserved task exceptions in the finalizer when we timeout.
            }
        }

        private async Task readMessageAsync(string endpoint, Stream stream, 
            Func<Message[], IMessageAcceptance> acceptMessages)
        {
            bool serializationErrorOccurred = false;
            Message[] messages = null;
            try
            {
                var length = await new ReadLength(Logger, endpoint).GetAsync(stream).ConfigureAwait(false);
                messages = await new ReadMessage(Logger, length, endpoint).GetAsync(stream).ConfigureAwait(false);
            }
            catch (SerializationException exception)
            {
                Logger.Info("Unable to deserialize messages", exception);
                serializationErrorOccurred = true;
            }

            if (serializationErrorOccurred)
            {
                await new WriteSerializationError(Logger).ProcessAsync(stream).ConfigureAwait(false);
            }

            IMessageAcceptance acceptance = null;
            byte[] errorBytes = null;
            try
            {
                acceptance = acceptMessages(messages);
                Logger.Debug("All messages from {0} were accepted", endpoint);
            }
            catch (QueueDoesNotExistsException)
            {
                Logger.Info("Failed to accept messages from {0} because queue does not exists", endpoint);
                errorBytes = ProtocolConstants.QueueDoesNoExiststBuffer;
            }
            catch (Exception exception)
            {
                errorBytes = ProtocolConstants.ProcessingFailureBuffer;
                Logger.Info("Failed to accept messages from " + endpoint, exception);
            }

            if (errorBytes != null)
            {
                await new WriteProcessingError(Logger, errorBytes, endpoint).ProcessAsync(stream).ConfigureAwait(false);
                return;
            }

            try
            {
                await new WriteReceived(Logger, endpoint).ProcessAsync(stream).ConfigureAwait(false);
            }
            catch (Exception)
            {
                acceptance.Abort();
                return;
            }

            try
            {
                await new ReadAcknowledgement(Logger, endpoint).ProcessAsync(stream).ConfigureAwait(false);
            }
            catch (Exception)
            {
                acceptance.Abort();
                return;
            }

            bool commitSuccessful;
            try
            {
                acceptance.Commit();
                commitSuccessful = true;
            }
            catch (Exception exception)
            {
                Logger.Info("Unable to commit messages from " + endpoint, exception);
                commitSuccessful = false;
            }

            if (commitSuccessful == false)
            {
                await new WriteRevert(Logger, endpoint).ProcessAsync(stream).ConfigureAwait(false);
            }
        }
    }
}