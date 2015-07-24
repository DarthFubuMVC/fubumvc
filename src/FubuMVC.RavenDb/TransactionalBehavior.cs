using System;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.RavenDb.RavenDb;

namespace FubuMVC.RavenDb
{
    public class TransactionalBehavior : WrappingBehavior
    {
        private readonly ISessionBoundary _session;
        private readonly ILogger _logger;
        private readonly IHttpRequest _httpRequest;

        public TransactionalBehavior(ISessionBoundary session, ILogger logger, IHttpRequest httpRequest)
        {
            _session = session;
            _logger = logger;
            _httpRequest = httpRequest;
        }

        protected override void invoke(Action action)
        {
            action();
            _session.SaveChanges();
            _session.WithOpenSession(x =>
                _logger.DebugMessage(() =>
                    TransactionalBehaviorRavenSessionUsageMessage.For(x, _httpRequest)));
        }
    }
}