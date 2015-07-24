using FubuCore.Dates;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Auditing;

namespace FubuMVC.RavenDb.Membership
{
    public class PersistedLoginAuditor : ILoginAuditor
    {
        private readonly ISystemTime _systemTime;
        private readonly ITransaction _transaction;

        public PersistedLoginAuditor(ISystemTime systemTime, ITransaction transaction)
        {
            _systemTime = systemTime;
            _transaction = transaction;
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

            _transaction.Execute<LoginAuditPersistor>(x => x.LogFailure(request, audit));
        }


        private void logSuccess(LoginRequest request)
        {
            var audit = new Audit
            {
                Message = new LoginSuccess {UserName = request.UserName},
                Timestamp = _systemTime.UtcNow()
            };

            _transaction.Execute<LoginAuditPersistor>(x => x.LogSuccess(request, audit));
        }


        public void ApplyHistory(LoginRequest request)
        {
            _transaction.Execute<LoginAuditPersistor>(x => x.ApplyHistory(request));
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

            _transaction.WithRepository(repo => repo.Update(audit));
        }
    }
}