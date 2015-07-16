using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuMVC.Core.Services.Remote;

namespace FubuMVC.Core.Services.Messaging.Tracking
{
    public static class MessageHistory
    {
        private readonly static IList<MessageTrack> _sent = new List<MessageTrack>();
        private readonly static IList<MessageTrack> _received = new List<MessageTrack>();
        private readonly static IList<MessageTrack> _outstanding = new List<MessageTrack>();
        
        private readonly static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private static MessageTrackListener _listener;

        private static readonly IList<IMessagingHub> _hubs = new List<IMessagingHub>(); 

        public static void StartListening(params RemoteServiceRunner[] runners)
        {
            ClearAll();

            _hubs.AddRange(runners.Select(x => x.Messaging));
            _hubs.Add(EventAggregator.Messaging);
            _listener = new MessageTrackListener();

            _hubs.Each(x => x.AddListener(_listener));
        }

        public static void ClearAll()
        {
            ClearHistory();

            if (_listener != null)
            {
                _hubs.Each(x => { if (_listener != null) x.RemoveListener(_listener); });
                
            }

            _hubs.Clear();
        }

        public static void ClearHistory()
        {
            _lock.Write(() => {
                _sent.Clear();
                _received.Clear();
                _outstanding.Clear();
            });
        }


        public static void Record(MessageTrack track)
        {
            _lock.Write(() => {
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

            _lock.Read(() => {
                if (!_outstanding.Any())
                {
                    EventAggregator.SendMessage(new AllMessagesComplete());
                }

                return true;
            });


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



        public class MessageTrackListener : IListener<MessageTrack>
        {
            public void Receive(MessageTrack message)
            {
                Record(message);
            }
        }

        public static bool WaitForWorkToFinish(Action action, int timeoutMilliseconds = 5000)
        {
            ClearHistory();
            action();
            return Wait.Until(() => !Outstanding().Any() && All().Any(), timeoutInMilliseconds: timeoutMilliseconds);
        }
    }


}