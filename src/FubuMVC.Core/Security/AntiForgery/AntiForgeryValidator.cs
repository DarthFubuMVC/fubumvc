using System;
using System.Collections.Specialized;
using System.Web;
using FubuCore.Binding;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgeryValidator : IAntiForgeryValidator
    {
        private readonly IRequestData _requestData;
        private readonly ISecurityContext _securityContext;
        private readonly IAntiForgerySerializer _serializer;
        private readonly IAntiForgeryTokenProvider _tokenProvider;

        public AntiForgeryValidator(IAntiForgeryTokenProvider tokenProvider, IAntiForgerySerializer serializer,
                                    IRequestData requestData, ISecurityContext securityContext)
        {
            _tokenProvider = tokenProvider;
            _serializer = serializer;
            _requestData = requestData;
            _securityContext = securityContext;
        }

        public bool Validate(string salt)
        {
            var cookies = (HttpCookieCollection) _requestData.Value("Cookies");
            var applicationPath = (string) _requestData.Value("ApplicationPath");
            var form = (NameValueCollection) _requestData.Value("Form");
            string fieldName = _tokenProvider.GetTokenName();
            string cookieName = _tokenProvider.GetTokenName(applicationPath);

            HttpCookie cookie = cookies[cookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return false;
            }
            AntiForgeryData cookieToken = _serializer.Deserialize(cookie.Value);

            string formValue = form[fieldName];
            if (string.IsNullOrEmpty(formValue))
            {
                return false;
            }
            AntiForgeryData formToken = _serializer.Deserialize(formValue);

            if (!string.Equals(cookieToken.Value, formToken.Value, StringComparison.Ordinal))
            {
                return false;
            }

            string currentUsername = AntiForgeryData.GetUsername(_securityContext.CurrentUser);
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