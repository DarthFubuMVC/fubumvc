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
            var cookie = new Cookie(FlashKey, json);

            _writer.AppendCookie(cookie);
        }

        public T Retrieve<T>()
        {
            if (!_cookies.Has(FlashKey))
            {
                return default(T);
            }

            var json = _cookies.GetValue(FlashKey);
            json = HttpUtility.UrlDecode(json);

            var cookie = new Cookie(FlashKey)
            {
                Expires = _systemTime.UtcNow().AddYears(-1)
            };

            _writer.AppendCookie(cookie);

            return JsonUtil.Get<T>(json);
        }

        public virtual string ToJson(object flashObject)
        {
            var value = JsonUtil.ToJson(flashObject);
            return HttpUtility.UrlEncode(value);
        }
    }
}