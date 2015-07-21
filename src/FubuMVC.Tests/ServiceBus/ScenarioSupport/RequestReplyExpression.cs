namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class RequestReplyExpression<T> : IReplyExpectation where T : Message, new()
    {
        private readonly NodeConfiguration _sender;
        private readonly Scenario _scenario;
        private readonly string _description;

        public RequestReplyExpression(NodeConfiguration sender, Scenario scenario, string description)
        {
            _sender = sender;
            _scenario = scenario;
            _description = description;
        }

        public class ReplyExpectation<TResponse> : IFromNode where TResponse : Message, new()
        {
            private readonly RequestReplyExpression<T> _parent;

            public ReplyExpectation(RequestReplyExpression<T> parent)
            {
                _parent = parent;
            }

            public void From(NodeConfiguration responder)
            {
                var step = new RequestReplyStep<T, TResponse>(_parent._description, _parent._sender, responder);
                _parent._scenario.AddStep(step);
            }
        }

        IFromNode IReplyExpectation.ExpectReply<TReply>()
        {
            return new ReplyExpectation<TReply>(this);
        }
    }



    public interface IReplyExpectation
    {
        IFromNode ExpectReply<T>() where T : Message, new();
    }

    public interface IFromNode
    {
        void From(NodeConfiguration responder);
    }
}