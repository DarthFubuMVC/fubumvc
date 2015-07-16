using System;

namespace FubuMVC.Core.Services.Messaging
{
    public static class MessagingHubExtensions
    {
        public static T WaitForMessage<T>(this IMessagingHub hub, Func<T, bool> filter, Action action, int wait = 5000)
        {
            var condition = new MessageWaitCondition<T>(filter);
            hub.AddListener(condition);
            action();

            try
            {
                return condition.Wait(wait);
            }
            finally
            {
                hub.RemoveListener(condition);
            }
        }

        public static T WaitForMessage<T>(this IMessagingHub hub, Action action, int wait = 5000)
        {
            Func<T, bool> filter = x => true;
            return hub.WaitForMessage(filter, action, wait);
        }
    }
}