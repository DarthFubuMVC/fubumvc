using System;
using System.Threading;
using System.Web;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgeryValidator : IAntiForgeryValidator
    {
        private readonly ICookies _cookies;
        private readonly IFubuApplicationFiles _fubuApplicationFiles;
        private readonly IRequestData _requestData;
        private readonly IAntiForgerySerializer _serializer;
        private readonly IAntiForgeryTokenProvider _tokenProvider;

        public AntiForgeryValidator(IAntiForgeryTokenProvider tokenProvider, IAntiForgerySerializer serializer,
            ICookies cookies, IFubuApplicationFiles fubuApplicationFiles,
            IRequestData requestData)
        {
            _tokenProvider = tokenProvider;
            _serializer = serializer;
            _cookies = cookies;
            _fubuApplicationFiles = fubuApplicationFiles;
            _requestData = requestData;
        }

        public bool Validate(string salt)
        {
            var applicationPath = _fubuApplicationFiles.RootPath;
            var fieldName = _tokenProvider.GetTokenName();
            var cookieName = _tokenProvider.GetTokenName(applicationPath);

            var cookie = _cookies.Get(cookieName);
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return false;
            }

            var cookieToken = _serializer.Deserialize(HttpUtility.UrlDecode(cookie.Value));

            var formValue = _requestData.ValuesFor(RequestDataSource.Header).Get(fieldName) as string
                            ??
                            _requestData.ValuesFor(RequestDataSource.Request).Get(fieldName) as string;

            if (formValue.IsEmpty())
            {
                return false;
            }

            var formToken = _serializer.Deserialize(formValue);

            if (!string.Equals(cookieToken.Value, formToken.Value, StringComparison.Ordinal))
            {
                return false;
            }

            var currentUsername = AntiForgeryData.GetUsername(Thread.CurrentPrincipal);

            if (!string.Equals(formToken.Username, currentUsername, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(salt ?? string.Empty, formToken.Salt, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }
    }
}