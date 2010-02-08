using FubuMVC.Core;
using FubuMVC.HelloWorld.Services;

namespace FubuMVC.HelloWorld.Controllers.Login
{
    public class LoggedInStatusController
    {
        private readonly IHttpSession _session;

        public LoggedInStatusController(IHttpSession session)
        {
            _session = session;
        }

        [FubuPartial]
        public LoggedInStatusViewModel Status(LoggedInStatusRequest request)
        {
            var status = _session[CurrentLoginStatus.Key] as CurrentLoginStatus;

            return new LoggedInStatusViewModel
                {
                    IsLoggedIn = status != null,
                    UserName = status == null ? "" : status.UserName
                };
            
        }
    }

    public class CurrentLoginStatus
    {
        public const string Key = "CURRENT_LOGIN_STATUS";

        public string UserName { get; set; }   
    }

    public class LoggedInStatusRequest
    {
    }

    public class LoggedInStatusViewModel
    {
        public bool IsLoggedIn { get; set; }
        public string UserName { get; set; }
    }
}