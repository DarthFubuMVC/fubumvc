using System;
using FubuCore.Dates;
using FubuMVC.Core.Security.Authentication;
using StoryTeller;
using StoryTeller.Grammars.Decisions;

namespace FubuMVC.IntegrationTesting.Fixtures
{
    public class LockedOutLogicFixture : Fixture
    {
        public LockedOutLogicFixture()
        {
            Title = "User Locked Out Logic";

            this["Logic"] = new LoginLockoutGrammar();
        }
    }

    public class LoginLockoutGrammar : DecisionTableGrammar
    {
        private readonly AuthenticationSettings _settings = new AuthenticationSettings();
        private readonly LoginRequest _request = new LoginRequest();
        private readonly ISystemTime _systemTime = SystemTime.Default();
        private readonly LockedOutRule _lockedOutRule;

        private DateTime toTime(string time)
        {
            time = time.Replace("NOW", "");
            return _systemTime.UtcNow().AddMinutes(int.Parse(time));
        }

        public LoginLockoutGrammar() : base("Locked out rules")
        {
            _lockedOutRule = new LockedOutRule(_settings, _systemTime);
        }

        public int NumberOfTries
        {
            set { _request.NumberOfTries = value; }
        }

        public int MaximumNumberOfFailedAttempts
        {
            set { _settings.MaximumNumberOfFailedAttempts = value; }
        }

        public int CoolingOffPeriod
        {
            set { _settings.CooloffPeriodInMinutes = value; }
        }

        public string CurrentLockedOutTime
        {
            set
            {
                if (value == "NONE") return;

                var time = toTime(value);
                _request.LockedOutUntil = time;
                _request.Status = LoginStatus.LockedOut;
            }
        }

        public bool IsLockedOut
        {
            get { return _lockedOutRule.IsLockedOut(_request) == LoginStatus.LockedOut; }
        }

        public string LockedOutUntil
        {
            get
            {
                _lockedOutRule.ProcessFailure(_request);

                if (_request.LockedOutUntil == null)
                {
                    return "NONE";
                }

                var totalMinutes = _request.LockedOutUntil.Value.Subtract(_systemTime.UtcNow()).TotalMinutes;
                var number = (int) Math.Round(totalMinutes);

                return number < 0 ? "NOW" + number : "NOW+" + number;
            }
        }
    }
}