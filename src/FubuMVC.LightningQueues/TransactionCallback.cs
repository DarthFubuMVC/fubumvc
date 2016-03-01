using System;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using LightningQueues;

namespace FubuMVC.LightningQueues
{
    public class TransactionCallback : IMessageCallback
    {
        private readonly Message _message;
        private readonly IQueueContext _context;

        public TransactionCallback(IQueueContext context, Message message)
        {
            _context = context;
            _message = message;
        }

        public void MarkSuccessful()
        {
            _context.SuccessfullyReceived();
            _context.CommitChanges();
        }

        public void MarkFailed(Exception ex)
        {
            _context.ReceiveLater(DateTimeOffset.Now);
        }

        public void MoveToDelayedUntil(DateTime time)
        {
            _context.ReceiveLater(time - DateTime.Now);
        }

        public void MoveToErrors(ErrorReport report)
        {
            var message = new Message
            {
                Id = _message.Id,
                Data = report.Serialize(),
                //Headers = report.Headers
            };

            message.Headers.Add("ExceptionType", report.ExceptionType);
            message.Queue = LightningQueuesTransport.ErrorQueueName;

            _context.Enqueue(message);
            MarkSuccessful();
        }

        public void Requeue()
        {
            var copy = _message.Copy();
            copy.Id = MessageId.GenerateRandom();
            copy.Queue = _message.Queue;
            _context.Enqueue(copy);
            MarkSuccessful();
        }
    }
}