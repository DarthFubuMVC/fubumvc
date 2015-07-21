using System;

namespace FubuMVC.Core.ServiceBus.Runtime.Cascading
{
    public class DelayedResponse : ISendMyself
    {
        private readonly object _outgoing;
        private readonly DateTime _time;
        private readonly TimeSpan _delay;

        public DelayedResponse(object outgoing, TimeSpan delay) : this(outgoing, DateTime.UtcNow.Add(delay))
        {
            _delay = delay;
        }

        public DelayedResponse(object outgoing, DateTime time)
        {
            _outgoing = outgoing;
            _time = time;
        }

        public object Outgoing
        {
            get { return _outgoing; }
        }

        public DateTime Time
        {
            get { return _time; }
        }

        public TimeSpan Delay
        {
            get { return _delay; }
        }

        public Envelope CreateEnvelope(Envelope original)
        {
            var outgoing = original.ForResponse(_outgoing);
            outgoing.ExecutionTime = Time;

            return outgoing;
        }

        public override string ToString()
        {
            return string.Format("Execute {0} at time: {1}", _outgoing, _time);
        }
    }
}