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

		public AntiForgeryData GetCookieToken()
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

		public AntiForgeryData SetCookieToken(string path, string domain)
		{
			var token = GetCookieToken();
			string name = _tokenProvider.GetTokenName(_httpContext.Request.ApplicationPath);
			string cookieValue = _serializer.Serialize(token);

			var newCookie = new HttpCookie(name, cookieValue) { HttpOnly = true, Domain = domain };
			if (!string.IsNullOrEmpty(path))
			{
				newCookie.Path = path;
			}
			_httpContext.Response.Cookies.Set(newCookie);

			return token;
		}

		public FormToken GetFormToken(AntiForgeryData token, string salt)
		{
            var formToken = new AntiForgeryData(token) {
                Salt = salt,
                Username = AntiForgeryData.GetUsername(_httpContext.User)
            };
            var tokenString = _serializer.Serialize(formToken);

			return new FormToken
			{
				Name = _tokenProvider.GetTokenName(),
				TokenString = tokenString
			};
		}
	}
}