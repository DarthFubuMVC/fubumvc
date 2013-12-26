using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Samples
{
    // SAMPLE: passing-state-with-ifuburequest
    public class PrivateMessage
    {
        public string Name { get; set; }
    }

    public class MyFirstBehavior : BasicBehavior
    {
        private readonly IFubuRequest _request;

        // As usual with FubuMVC, IFubuRequest is generally
        // going to be resolved from the IoC container
        // through constructor injection
        public MyFirstBehavior(IFubuRequest request) : base(PartialBehavior.Executes)
        {
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            // create the PrivateMessage object
            var message = createPrivateMessage();

            // store the PrivateMessage in 
            // IFubuRequest where other Behavior's
            // could find it later
            _request.Set(message);

            return DoNext.Continue;
        }

        private static PrivateMessage createPrivateMessage()
        {
            return new PrivateMessage{Name = "Han Solo"};
        }
    }

    public class MySecondBehavior : BasicBehavior
    {
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;

        public MySecondBehavior(IFubuRequest request, IOutputWriter writer)
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _writer = writer;
        }

        protected override DoNext performInvoke()
        {
            // Extract the PrivateMessage to render.
            var message = _request.Get<PrivateMessage>();
            _writer.Write(MimeType.Text, message.Name);

            return DoNext.Continue;
        }
    }
    // ENDSAMPLE

    public static class FubuRequestSamples
    {
        public static void UsingInMemoryFubuRequest()
        {
            // SAMPLE: using-in-memory-fubu-request
            var request = new InMemoryFubuRequest();
            request.Set(new PrivateMessage{Name = "Darth Vader"});
            // ENDSAMPLE
        }
    }

}