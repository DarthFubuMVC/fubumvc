using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.Services.Remote;

namespace FubuMVC.Core.Services.Messaging.Tracking
{
    public static class MessageHistory
    {
        private static readonly IList<MessageTrack> _sent = new List<MessageTrack>();
        private static readonly IList<MessageTrack> _received = new List<MessageTrack>();
        private static readonly IList<MessageTrack> _outstanding = new List<MessageTrack>();

        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private static MessageTrackListener _listener;

        private static readonly IList<IMessagingHub> _hubs = new List<IMessagingHub>();

        static MessageHistory()
        {
            ClearAll();
        }


        public static void ConnectRemoteListeners(params RemoteServiceRunner[] runners)
        {
            ClearAll();

            var hubs = runners.Select(x => x.Messaging).ToArray();
            hubs.Each(x => x.AddListener(_listener));
            _hubs.AddRange(hubs);
        }

        /// <summary>
        /// This completely resets MessageHistory tracking and will disconnect
        /// any remote listeners
        /// </summary>
        public static void ClearAll()
        {
            ClearHistory();

            if (_listener != null)
            {
                _hubs.Each(x =>
                {
                    if (_listener != null) x.RemoveListener(_listener);
                });
            }

            _hubs.Clear();

            _hubs.Add(GlobalMessageTracking.Messaging);

            _listener = new MessageTrackListener();
            GlobalMessageTracking.Messaging.AddListener(_listener);
        }

        public static void ClearHistory()
        {
            _lock.Write(() =>
            {
                _sent.Clear();
                _received.Clear();
                _outstanding.Clear();
            });
        }


        public static void Record(MessageLogRecord record)
        {
            var messageTrack = record.ToMessageTrack();
            if (messageTrack != null) Record(messageTrack);
        }

        public static void Record(MessageTrack track)
        {
            _lock.Write(() =>
            {
                Debug.WriteLine("Track: " + track);


                if (track.Status == MessageTrack.Sent)
                {
                    _sent.Add(track);
                    _outstanding.Add(track);
                }
                else
                {
                    _received.Add(track);
                    _outstanding.Remove(track);
                }
            });

            _lock.Read(() =>
            {
                if (!_outstanding.Any())
                {
                    GlobalMessageTracking.SendMessage(new AllMessagesComplete());
                }

                return true;
            });
        }

        public static void AssertFinished()
        {
            if (_outstanding.Any())
            {
                var writer = new StringWriter();

                writer.WriteLine("There are outstanding messages that did not complete in time!");
                writer.WriteLine("Outstanding message tracks:");
                _outstanding.Each(x => writer.WriteLine(x));

                writer.WriteLine();
                writer.WriteLine("Sent");
                _sent.Each(x => writer.WriteLine(x));

                writer.WriteLine();
                writer.WriteLine("Received");
                _sent.Each(x => writer.WriteLine(x));
            }
        }

        public static IEnumerable<MessageTrack> Received()
        {
            return _lock.Read(() => _received.ToArray());
        }

        public static IEnumerable<MessageTrack> Outstanding()
        {
            return _lock.Read(() => _outstanding.ToArray());
        }

        public static IEnumerable<MessageTrack> All()
        {
            return _lock.Read(() => _sent.Union(_received).ToList());
        }

        public static bool WaitForWorkToFinish(Action action, int timeoutMilliseconds = 5000)
        {
            ClearHistory();
            action();
            return Wait.Until(() => !Outstanding().Any() && All().Any(), timeoutInMilliseconds: timeoutMilliseconds);
        }


        public class MessageTrackListener : IListener<MessageTrack>
        {
            public void Receive(MessageTrack message)
            {
                Record(message);
            }
        }
    }
}