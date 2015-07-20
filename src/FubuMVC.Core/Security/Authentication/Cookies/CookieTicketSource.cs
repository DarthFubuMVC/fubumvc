using System;
using System.Web;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Security.Authentication.Tickets;
using HtmlTags;

namespace FubuMVC.Core.Security.Authentication.Cookies
{
    public class CookieTicketSource : ITicketSource
    {
        private readonly ISystemTime _systemTime;
        private readonly IEncryptor _encryptor;
        private readonly ILoginCookieService _cookies;
        private readonly ILogger _logger;

        public CookieTicketSource(ISystemTime systemTime, IEncryptor encryptor, ILoginCookieService cookies, ILogger logger)
        {
            _systemTime = systemTime;
            _encryptor = encryptor;
            _cookies = cookies;
            _logger = logger;
        }

        public AuthenticationTicket CurrentTicket()
        {
            try
            {
                var cookie = _cookies.Current();
                if (cookie != null)
                {
                    var json = DecodeJson(cookie);
                    return JsonUtil.Get<AuthenticationTicket>(json);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.Error("failed while trying to retrieve the user cookie", ex);
                return null;
            }
        }

        public string EncodeJson(AuthenticationTicket ticket)
        {
            var value = _encryptor.Encrypt(JsonUtil.ToJson(ticket));
            return HttpUtility.UrlEncode(value);
        }

        public string DecodeJson(Cookie cookie)
        {
            var json = cookie.Value;
            var value = HttpUtility.UrlDecode(json);

            return _encryptor.Decrypt(value);
        }

        public void Persist(AuthenticationTicket ticket)
        {
            var cookie = _cookies.CreateCookie(_systemTime);
            cookie.Value = EncodeJson(ticket);

            _cookies.Update(cookie);
        }

        public void Delete()
        {
            var cookie = _cookies.CreateCookie(_systemTime);
            cookie.Expires = _systemTime.UtcNow().AddYears(-1);

            _cookies.Update(cookie);
        }
    }
}