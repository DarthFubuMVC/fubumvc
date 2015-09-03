using System;
using LightningQueues.Internal;
using LightningQueues.Model;

namespace LightningQueues
{
    public static class QueueExtensions
    {
        public static Message Receive(this IQueueManager queueManager, string queueName)
        {
            return queueManager.Receive(queueName, null, TimeSpan.FromDays(1));
        }

        public static Message Receive(this IQueueManager queueManager, string queueName, TimeSpan timeout)
        {
            return queueManager.Receive(queueName, null, timeout);
        }

        public static Message Receive(this IQueueManager queueManager, string queueName, string subqueue)
        {
            return queueManager.Receive(queueName, subqueue, TimeSpan.FromDays(1));
        }

        public static Message Peek(this IQueueManager queueManager, string queueName)
        {
            return queueManager.Peek(queueName, null, TimeSpan.FromDays(1));
        }

        public static Message Peek(this IQueueManager queueManager, string queueName, TimeSpan timeout)
        {
            return queueManager.Peek(queueName, null, timeout);
        }

        public static Message Peek(this IQueueManager queueManager, string queueName, string subqueue)
        {
            return queueManager.Peek(queueName, subqueue, TimeSpan.FromDays(1));
        }

        public static Message Receive(this IQueueManager queueManager, ITransaction transaction, string queueName)
        {
            return queueManager.Receive(transaction, queueName, null, TimeSpan.FromDays(1));
        }

        public static Message Receive(this IQueueManager queueManager, ITransaction transaction, string queueName, TimeSpan timeout)
        {
            return queueManager.Receive(transaction, queueName, null, timeout);
        }

        public static Message Receive(this IQueueManager queueManager, ITransaction transaction, string queueName, string subqueue)
        {
            return queueManager.Receive(transaction, queueName, subqueue, TimeSpan.FromDays(1));
        }

        public static Message Receive(this ITransactionalScope scope, string queue)
        {
            return scope.QueueManager.Receive(scope.Transaction, queue);
        }

        public static Message Receive(this ITransactionalScope scope, string queue, TimeSpan timeout)
        {
            return scope.QueueManager.Receive(scope.Transaction, queue, timeout);
        }

        public static Message Receive(this ITransactionalScope scope, string queue, string subqueue)
        {
            return scope.QueueManager.Receive(scope.Transaction, queue);
        }

        public static Message Receive(this ITransactionalScope scope, string queue, string subqueue, TimeSpan timeout)
        {
            return scope.QueueManager.Receive(scope.Transaction, queue, subqueue, timeout);
        }

        public static Message ReceiveById(this ITransactionalScope scope, string queue, MessageId id)
        {
            return scope.QueueManager.ReceiveById(scope.Transaction, queue, id);
        }

        public static MessageId Send(this ITransactionalScope scope, Uri uri, MessagePayload payload)
        {
            return scope.QueueManager.Send(scope.Transaction, uri, payload);
        }

        public static void EnqueueDirectlyTo(this ITransactionalScope scope, string queue, string subQueue, MessagePayload payload, MessageId id = null)
        {
            scope.QueueManager.EnqueueDirectlyTo(scope.Transaction, queue, subQueue, payload, id);
        }

        public static void EnqueueDirectlyTo(this ITransactionalScope scope, string queue, MessagePayload payload, MessageId id = null)
        {
            scope.QueueManager.EnqueueDirectlyTo(scope.Transaction, queue, null, payload, id);
        }
    }
}