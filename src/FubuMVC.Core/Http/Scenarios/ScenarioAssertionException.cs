using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using FubuCore;

namespace FubuMVC.Core.Http.Scenarios
{
    [Serializable]
    public class ScenarioAssertionException : Exception
    {
        private readonly IList<string> _messages = new List<string>();

        public ScenarioAssertionException()
        {
        }

        protected ScenarioAssertionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public void Add(string format, params object[] parameters)
        {
            _messages.Add(format.ToFormat(parameters));
        }

        public void AssertValid()
        {
            if (_messages.Any())
            {
                throw this;
            }
        }

        public override string Message
        {
            get
            {
                var writer = new StringWriter();
                _messages.Each(x => writer.WriteLine(x));

                if (Body.IsNotEmpty())
                {
                    writer.WriteLine();
                    writer.WriteLine();
                    writer.WriteLine("Actual body text was:");
                    writer.WriteLine();
                    writer.WriteLine(Body);
                }

                return writer.ToString();
            }
        }

        public string Body { get; set; }
    }
}