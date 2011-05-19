using System.Web;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgeryService : IAntiForgeryService
    {
        private readonly IOutputWriter _outputWriter;
        private readonly IRequestData _requestData;
        private readonly ISecurityContext _securityContext;
        private readonly IAntiForgerySerializer _serializer;
        private readonly IAntiForgeryTokenProvider _tokenProvider;

        public AntiForgeryService(IOutputWriter outputWriter,
                                  IAntiForgeryTokenProvider tokenProvider,
                                  IAntiForgerySerializer serializer,
                                  ISecurityContext securityContext,
                                  IRequestData requestData)
        {
            _outputWriter = outputWriter;
            _tokenProvider = tokenProvider;
            _serializer = serializer;
            _securityContext = securityContext;
            _requestData = requestData;
        }

        public AntiForgeryData SetCookieToken(string path, string domain)
        {
            var applicationPath = (string) _requestData.Value("ApplicationPath");
            AntiForgeryData token = GetCookieToken();
            string name = _tokenProvider.GetTokenName(applicationPath);
            string cookieValue = _serializer.Serialize(token);

            var newCookie = new HttpCookie(name, cookieValue) {HttpOnly = true, Domain = domain};
            if (!string.IsNullOrEmpty(path))
            {
                newCookie.Path = path;
            }
            _outputWriter.AppendCookie(newCookie);

            return token;
        }

        public FormToken GetFormToken(AntiForgeryData token, string salt)
        {
            var formToken = new AntiForgeryData(token)
            {
                Salt = salt,
                Username = AntiForgeryData.GetUsername(_securityContext.CurrentUser)
            };
            string tokenString = _serializer.Serialize(formToken);

            return new FormToken
            {
                Name = _tokenProvider.GetTokenName(),
                TokenString = tokenString
            };
        }

        public AntiForgeryData GetCookieToken()
        {
            var cookies = (HttpCookieCollection) _requestData.Value("Cookies");
            var applicationPath = (string) _requestData.Value("ApplicationPath");
            string name = _tokenProvider.GetTokenName(applicationPath);
            HttpCookie cookie = cookies[name];
            AntiForgeryData cookieToken = null;
            if (cookie != null)
            {
                try
                {
                    cookieToken = _serializer.Deserialize(cookie.Value);
                }
                catch (FubuException)
                {
                    // TODO -- log this.  Need a generic tracing mechanism
                }
            }

            return cookieToken ?? _tokenProvider.GenerateToken();
        }
    }
}