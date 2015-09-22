using System;
using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;

namespace FubuMVC.IntegrationTesting.ServiceBus
{
    public class WebMessage
    {
        public string Text { get; set; }

        public void Assert()
        {
            if (Text.Contains("Bad"))
            {
                Text = "now it is good";

                throw new DivideByZeroException("it was Bad");
            }

            if (Text.Contains("Retry"))
            {
                Text = "now it is good";
                throw new InvalidOperationException("triggering retry now");
            }
        }
    }

    public class WebMessage2
    {
        public string Text { get; set; }
    }

    public class WebMessage3
    {
        public string Text { get; set; }
    }

    public class WebMessage4
    {
        public string Text { get; set; }
    }

    public class WebMessageTrace
    {
        public string Text { get; set; }
    }

    public class MessageRecorder
    {
        public IList<string> Messages = new List<string>();
    }

    public class TriggerImmediate
    {
        public string Text { get; set; }
        public string ContinueText { get; set; }
    }

    public class WebMessageHandler
    {
        private readonly MessageRecorder _recorder;

        public WebMessageHandler(MessageRecorder recorder)
        {
            _recorder = recorder;
        }

        public object Handle(TriggerImmediate message)
        {
            _recorder.Messages.Add(message.Text);

            return ContinueImmediately.With(new WebMessage {Text = message.ContinueText});
        }

        public void Handle(WebMessageTrace trace)
        {
            _recorder.Messages.Add(trace.Text);
        }

        public WebMessageTrace Trace(WebMessage message)
        {
            return new WebMessageTrace {Text = "Traced: " + message.Text};
        }

        public WebMessage2 Handle(WebMessage message)
        {
            message.Assert();

            _recorder.Messages.Add(message.Text);

            return new WebMessage2
            {
                Text = message.Text + "-2"
            };
        }

        public object[] Handle(WebMessage2 message)
        {
            _recorder.Messages.Add(message.Text);

            return new[] {new WebMessage3 {Text = message.Text + "-3"}, new WebMessage3 {Text = message.Text + "-4"}};
        }

        public void Handle(WebMessage3 message)
        {
            _recorder.Messages.Add(message.Text);
        }

        public void Handle(WebMessage4 message)
        {
            _recorder.Messages.Add(message.Text);
        }
    }
}