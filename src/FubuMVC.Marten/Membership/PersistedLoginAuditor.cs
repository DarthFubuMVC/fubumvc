using System;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Auditing;
using Marten;

namespace FubuMVC.Marten.Membership
{
    public class PersistedLoginAuditor : ILoginAuditor
    {
        private readonly ISystemTime _systemTime;
        private readonly IDocumentSession _session;
        private readonly ILogger _logger;
        private readonly LoginAuditPersistor _persistor;

        public PersistedLoginAuditor(ISystemTime systemTime, IDocumentSession session, ILogger logger, LoginAuditPersistor persistor)
        {
            _systemTime = systemTime;
            _session = session;
            _logger = logger;
            _persistor = persistor;
        }

        public void Audit(LoginRequest request)
        {
            if (request.Status == LoginStatus.Succeeded)
            {
                logSuccess(request);
            }
            else
            {
                logFailure(request);
            }
        }

        private void logFailure(LoginRequest request)
        {
            var audit = new Audit
            {
                Message = new LoginFailure {UserName = request.UserName},
                Timestamp = _systemTime.UtcNow()
            };
            try
            {

                _persistor.LogFailure(request, audit);
                _session.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error("Failed during login request auditing", e);
            }

        }


        private void logSuccess(LoginRequest request)
        {
            var audit = new Audit
            {
                Message = new LoginSuccess {UserName = request.UserName},
                Timestamp = _systemTime.UtcNow()
            };

            try
            {

                _persistor.LogSuccess(request, audit);
                _session.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error("Failed during login request auditing", e);
            }

        }


        public void ApplyHistory(LoginRequest request)
        {
            try
            {

                _persistor.ApplyHistory(request);
                _session.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error("Failed during login request auditing", e);
            }

        }


        public void Audit<T>(T log) where T : AuditMessage
        {
            persistAudit(log);
        }

        private void persistAudit(AuditMessage message)
        {
            var audit = new Audit
            {
                Message = message,
                Timestamp = _systemTime.UtcNow()
            };

            try
            {

                _session.Store(audit);
                _session.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error("Failed during login request auditing", e);
            }

        }
    }
}