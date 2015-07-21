using System;
using FubuCore.Logging;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.ServiceBus.Web
{
    public class SendMessageBehavior<T> : BasicBehavior, IMayHaveResourceType where T : class
    {
        private readonly IFubuRequest _request;
        private readonly IServiceBus _serviceBus;
        private readonly ILogger _logger;

        public SendMessageBehavior(IFubuRequest request, IServiceBus serviceBus, ILogger logger)
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _serviceBus = serviceBus;
            _logger = logger;
        }

        protected override DoNext performInvoke()
        {
            try
            {
                var message = _request.Get<T>();
                _serviceBus.Send(message);

                _request.Set(AjaxContinuation.Successful());
            }
            catch (Exception e)
            {
                _request.Set(new AjaxContinuation
                {
                    Success = false
                });

                _logger.Error("Error trying to publish message of type " + typeof(T).FullName, e);
            }

            return DoNext.Continue;
        }

        public Type ResourceType()
        {
            return typeof(AjaxContinuation);
        }
    }
}