using System;
using System.Text;
using System.Web;
using FubuCore.Dates;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.SessionState
{
    public class CookieFlashProvider : IFlash
    {
        public const string FlashKey = "fubuFlash";

        private readonly IOutputWriter _writer;
        private readonly ICookies _cookies;
        private readonly ISystemTime _systemTime;

        public CookieFlashProvider(IOutputWriter writer, ICookies cookies, ISystemTime systemTime)
        {
            _writer = writer;
            _cookies = cookies;
            _systemTime = systemTime;
        }

        public void Flash(object flashObject)
        {
            var json = ToJson(flashObject);
            var cookie = new Cookie(FlashKey, json)
            {
                Path = "/",
                Expires = _systemTime.UtcNow().AddDays(1)
            };

            _writer.AppendCookie(cookie);
        }

        public T Retrieve<T>()
        {
            if (!_cookies.Has(FlashKey))
            {
                return default(T);
            }

            var json = _cookies.GetValue(FlashKey);
            json = FromBase64String(HttpUtility.UrlDecode(json));

            var cookie = new Cookie(FlashKey)
            {
                Path = "/",
                Expires = _systemTime.UtcNow().AddYears(-1)
            };

            _writer.AppendCookie(cookie);

            return JsonUtil.Get<T>(json);
        }

        public virtual string ToJson(object flashObject)
        {
            var value = ToBase64String(JsonUtil.ToJson(flashObject));
            return HttpUtility.UrlEncode(value);
        }

        public static string ToBase64String( string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string FromBase64String(string input)
        {
            var bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}