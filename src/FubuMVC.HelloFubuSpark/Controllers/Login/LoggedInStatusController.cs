using FubuMVC.Core;
using FubuMVC.HelloFubuSpark.Services;

namespace FubuMVC.HelloFubuSpark.Controllers.Login
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
                    UserName = request.User_Agent + "--" + (status == null ? "" : status.UserName)
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
        // Will come in from request headers (i.e. "User-Agent")
        public string User_Agent { get; set; }

        // Will come in from request property (i.e. HttpContext.Current.Request.IsLocal)
        public bool IsLocal { get; set; }
    }

    public class LoggedInStatusViewModel
    {
        public bool IsLoggedIn { get; set; }
        public string UserName { get; set; }
    }
}