using System.Web;

namespace FubuMVC.Core.Security.AntiForgery
{
	public class AntiForgeryService : IAntiForgeryService
	{
		private readonly HttpContextBase _httpContext;
		private readonly IAntiForgeryTokenProvider _tokenProvider;
		private readonly IAntiForgerySerializer _serializer;

		public AntiForgeryService(HttpContextBase httpContext, IAntiForgeryTokenProvider tokenProvider, IAntiForgerySerializer serializer)
		{
			_httpContext = httpContext;
			_tokenProvider = tokenProvider;
			_serializer = serializer;
		}

		public AntiForgeryData GetCookieToken(string salt)
		{
			string name = _tokenProvider.GetTokenName(_httpContext.Request.ApplicationPath);
			HttpCookie cookie = _httpContext.Request.Cookies[name];
			AntiForgeryData cookieToken = null;
			if (cookie != null)
			{
				try
				{
					cookieToken = _serializer.Deserialize(cookie.Value);
				}
				catch (FubuException) { /*Ick!*/ }
			}

			if (cookieToken == null)
			{
				cookieToken = _tokenProvider.GenerateToken();
			}

			return cookieToken;
		}

		public void SetCookieToken(AntiForgeryData token, string path, string domain)
		{
			string name = _tokenProvider.GetTokenName(_httpContext.Request.ApplicationPath);
			string cookieValue = _serializer.Serialize(token);

			var newCookie = new HttpCookie(name, cookieValue) { HttpOnly = true, Domain = domain };
			if (!string.IsNullOrEmpty(path))
			{
				newCookie.Path = path;
			}
			_httpContext.Response.Cookies.Set(newCookie);
		}

		public string GetFormTokenString(AntiForgeryData token, string salt)
		{
            var formToken = new AntiForgeryData(token) {
                Salt = salt,
                Username = AntiForgeryData.GetUsername(_httpContext.User)
            };
            return _serializer.Serialize(formToken);
		}
	}
}