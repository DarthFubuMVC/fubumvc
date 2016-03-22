using FubuCore;
using FubuMVC.Core.Security.Authentication;
using Marten;

namespace FubuMVC.Marten.Membership
{
    public class LoginAuditPersistor
    {
        private readonly IDocumentSession _session;

        public LoginAuditPersistor(IDocumentSession session)
        {
            _session = session;
        }

        public void LogFailure(LoginRequest request, Audit audit)
        {
            if (request.UserName.IsEmpty()) return;

            _session.Store(audit);

            var history = _session.Load<LoginFailureHistory>(request.UserName) ?? new LoginFailureHistory
            {
                Id = request.UserName
            };

            history.Attempts = request.NumberOfTries;
            history.LockedOutTime = request.LockedOutUntil;

            _session.Store(history);
        }

        public void LogSuccess(LoginRequest request, Audit audit)
        {
            _session.Store(audit);

            var history = _session.Load<LoginFailureHistory>(request.UserName);
            if (history != null)
            {
                _session.Delete(history);
            }
        }

        public void ApplyHistory(LoginRequest request)
        {
            if (request.UserName.IsEmpty()) return;
            var history = _session.Load<LoginFailureHistory>(request.UserName);
            if (history == null) return;

            request.NumberOfTries = history.Attempts;
            request.LockedOutUntil = history.LockedOutTime;
        }
    }
}