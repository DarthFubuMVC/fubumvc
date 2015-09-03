using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using LightningQueues.Model;
using LightningQueues.Protocol;
using NLog;

namespace LightningQueues.Logging
{
    public class NLogLogger : ILogger
    {
        private readonly Logger _logger;

        public NLogLogger(Logger logger)
        {
            _logger = logger;
        }

        public void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void Debug(Func<string> message)
        {
            _logger.Debug(new LogMessageGenerator(message));
        }

        public void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public void Info(string message, Exception exception, params object[] args)
        {
            _logger.InfoException(message.ToFormat(args), exception);
        }

        public void Error(string message, Exception exception)
        {
            _logger.ErrorException(message, exception);
        }

        public void FailedToSend(Endpoint destination, string reason, Exception exception = null)
        {
            _logger.Info("Failed to send to {0}: {1}", destination, reason);
            _logger.DebugException("Details:", exception);
        }

        public void QueuedForReceive(Message message)
        {
            Debug("Message {0} queued for receive in queue {1}", message.Id, message.Queue);
        }

        public void QueuedForSend(Message message, Endpoint destination)
        {
            Debug("Message {0} queued for send to {1}", message.Id, destination);
        }

        public void MessageReceived(Message message)
        {
            Debug("Message {0} received", message.Id);
        }

        public void MessagesSent(IList<Message> messages, Endpoint destination)
        {
            Debug(() => "Messages {0} sent to {1}".ToFormat(string.Join(", ", messages.Select(x => x.Id)), destination));
        }
    }
}