using FubuCore.Dates;
using FubuMVC.Core.Security.Authentication.Endpoints;

namespace FubuMVC.Core.Security.Authentication
{
    public interface ILockedOutRule
    {
        LoginStatus IsLockedOut(LoginRequest request);
        void ProcessFailure(LoginRequest request);
    }

    public class LockedOutRule : ILockedOutRule
    {
        private readonly AuthenticationSettings _settings;
        private readonly ISystemTime _systemTime;

        public LockedOutRule(AuthenticationSettings settings, ISystemTime systemTime)
        {
            _settings = settings;
            _systemTime = systemTime;
        }

        public LoginStatus IsLockedOut(LoginRequest request)
        {
            if (request.NumberOfTries >= _settings.MaximumNumberOfFailedAttempts)
            {
                return LoginStatus.LockedOut;
            }

            if (request.LockedOutUntil != null)
            {
                if (request.LockedOutUntil.Value > _systemTime.UtcNow())
                {
                    return LoginStatus.LockedOut;
                }
            }

            return LoginStatus.NotAuthenticated;
        }

        public void ProcessFailure(LoginRequest request)
        {
            if (request.LockedOutUntil != null && request.LockedOutUntil.Value <= _systemTime.UtcNow())
            {
                request.LockedOutUntil = null;
            }

            if (IsLockedOut(request) == LoginStatus.LockedOut)
            {
                request.Status = LoginStatus.LockedOut;
                request.Message = LoginKeys.LockedOut.ToString();
                request.NumberOfTries = 0; // This is important for the unlocking later

                if (request.LockedOutUntil == null)
                {
                    request.LockedOutUntil = _systemTime.UtcNow().AddMinutes(_settings.CooloffPeriodInMinutes);
                }
            }
            else
            {
                request.Message = LoginKeys.Failed.ToFormat(request.NumberOfTries,
                                                            _settings.MaximumNumberOfFailedAttempts);
            }
        }

        
    }
}