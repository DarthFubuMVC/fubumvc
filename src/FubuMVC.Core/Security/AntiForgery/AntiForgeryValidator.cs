using System;
using System.Web;

namespace FubuMVC.Core.Security.AntiForgery
{
	public interface IAntiForgeryValidator
	{
		bool Validate(string salt);
	}

	public class AntiForgeryValidator : IAntiForgeryValidator
	{
		private readonly IAntiForgeryTokenProvider _tokenProvider;
		private readonly IAntiForgerySerializer _serializer;
		private readonly HttpContextBase _httpContext;

		public AntiForgeryValidator(IAntiForgeryTokenProvider tokenProvider, IAntiForgerySerializer serializer, HttpContextBase httpContext)
		{
			_tokenProvider = tokenProvider;
			_serializer = serializer;
			_httpContext = httpContext;
		}

		public bool Validate(string salt)
		{
			string fieldName = _tokenProvider.GetTokenName();
			string cookieName = _tokenProvider.GetTokenName(_httpContext.Request.ApplicationPath);

			HttpCookie cookie = _httpContext.Request.Cookies[cookieName];
			if (cookie == null || string.IsNullOrEmpty(cookie.Value))
			{
				return false;
			}
			AntiForgeryData cookieToken = _serializer.Deserialize(cookie.Value);

			string formValue = _httpContext.Request.Form[fieldName];
			if (string.IsNullOrEmpty(formValue))
			{
				return false;
			}
			AntiForgeryData formToken = _serializer.Deserialize(formValue);

			if (!string.Equals(cookieToken.Value, formToken.Value, StringComparison.Ordinal))
			{
				return false;
			}

			string currentUsername = AntiForgeryData.GetUsername(_httpContext.User);
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