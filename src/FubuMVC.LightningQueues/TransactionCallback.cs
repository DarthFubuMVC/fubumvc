using System;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using LightningQueues;
using LightningQueues.Model;

namespace FubuMVC.LightningQueues
{
    public class TransactionCallback : IMessageCallback
    {
        private readonly Message _message;
        private readonly IDelayedMessageCache<MessageId> _delayedMessages;
        private readonly ITransactionalScope _transaction;

        public TransactionCallback(ITransactionalScope transaction, Message message, IDelayedMessageCache<MessageId> delayedMessages)
        {
            _transaction = transaction;
            _message = message;
            _delayedMessages = delayedMessages;
        }

        public void MarkSuccessful()
        {
            _transaction.Commit();
        }

        public void MarkFailed(Exception ex)
        {
            _transaction.Rollback();
        }

        public void MoveToDelayedUntil(DateTime time)
        {
            _delayedMessages.Add(_message.Id, time);
            _transaction.EnqueueDirectlyTo(LightningQueuesTransport.DelayedQueueName, _message.ToPayload(), _message.Id);
            MarkSuccessful();
        }

        public void MoveToErrors(ErrorReport report)
        {
            var messagePayload = new MessagePayload
            {
                Data = report.Serialize(),
                Headers = report.Headers
            };

            messagePayload.Headers.Add("ExceptionType", report.ExceptionType);

            _transaction.EnqueueDirectlyTo(LightningQueuesTransport.ErrorQueueName, messagePayload, _message.Id);
            MarkSuccessful();
        }

        public void Requeue()
        {
            _transaction.EnqueueDirectlyTo(_message.Queue, _message.SubQueue, _message.ToPayload(), MessageId.GenerateRandom());
            MarkSuccessful();
        }
    }
}