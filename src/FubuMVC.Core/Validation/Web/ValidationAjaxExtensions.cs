using FubuMVC.Core.Ajax;

namespace FubuMVC.Core.Validation.Web
{
	public class ValidationOrigin
	{
		public static readonly string Client = "client";
		public static readonly string Server = "server";
	}

	public static class ValidationAjaxExtensions
	{
		private const string OriginKey = "validationOrigin";

		public static AjaxContinuation ValidationOrigin(this AjaxContinuation continuation, string origin)
		{
			continuation[OriginKey] = origin;
			return continuation;
		}

		public static string ValidationOrigin(this AjaxContinuation continuation)
		{
			if (continuation.HasData(OriginKey))
			{
				return continuation[OriginKey].ToString();
			}

			return Web.ValidationOrigin.Server;
		}
	}
}