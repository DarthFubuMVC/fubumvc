using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.Runtime.Cascading
{
    public class Respond : ISendMyself
    {
        private readonly object _message;
        private readonly IList<Action<Envelope, Envelope>> _actions = new List<Action<Envelope, Envelope>>();
        private string _description = string.Empty;
        private bool _sentToSender;

        public static Respond With(object message)
        {
            return new Respond(message);
        }

        private Respond(object message)
        {
            if (message == null) throw new ArgumentNullException("message");
            _description = "Respond '{0}'".ToFormat(message);

            _message = message;
        }

        private Action<Envelope, Envelope> alter
        {
            set
            {
                _actions.Add(value);
            }
        }

        public Respond WithHeader(string key, string value)
        {
            alter = (_, e) => e.Headers[key] = value;
            _description += "; {0}='{1}'".ToFormat(key, value);

            return this;
        }

        public Respond ToSender()
        {
            alter = (old, @new) => @new.Destination = old.ReplyUri;
            _description += "; respond to sender";

            _sentToSender = true;

            return this;
        }

        public void AssertWasSentBackToSender()
        {
            if (!_sentToSender)
            {
                throw new Exception("Was NOT sent back to the sender");
            }
        }

        public Respond To(Uri destination)
        {
            alter = (_, e) => e.Destination = destination;
            _description += "; Destination=" + destination;

            return this;
        }

        public Respond DelayedUntil(DateTime time)
        {
            alter = (_, e) => e.ExecutionTime = time.ToUniversalTime();
            _description += "; Delayed until " + time.ToUniversalTime();

            return this;
        }

        public Respond DelayedBy(TimeSpan timeSpan)
        {
            return DelayedUntil(DateTime.UtcNow.Add(timeSpan));
        }

        public Respond Altered(Action<Envelope> alteration)
        {
            alter = (_, e) => alteration(e);
            return this;
        }

        public Envelope CreateEnvelope(Envelope original)
        {
            var envelope = original.ForResponse(_message);

            _actions.Each(x => x(original, envelope));

            return envelope;
        }

        public override string ToString()
        {
            return _description;
        }
    }
}