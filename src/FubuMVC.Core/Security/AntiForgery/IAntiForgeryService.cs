namespace FubuMVC.Core.Security.AntiForgery
{
	public interface IAntiForgeryService
	{
		AntiForgeryData GetCookieToken(string salt);
		void SetCookieToken(AntiForgeryData token, string path, string domain);
		string GetFormTokenString(AntiForgeryData token, string salt);
	}
}