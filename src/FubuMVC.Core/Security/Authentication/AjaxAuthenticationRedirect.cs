using System.Net;
using FubuCore.Binding;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.Core.Security.Authentication
{
	public class AjaxAuthenticationRedirect : IAuthenticationRedirect
	{
		private readonly IRequestData _data;
		private readonly IOutputWriter _outputWriter;
		private readonly IUrlRegistry _urls;

		public AjaxAuthenticationRedirect(IRequestData data, IOutputWriter outputWriter, IUrlRegistry urls)
		{
			_data = data;
			_outputWriter = outputWriter;
			_urls = urls;
		}

		public bool Applies()
		{
			return _data.IsAjaxRequest();
		}

		public FubuContinuation Redirect()
		{
			var continuation = BuildAjaxContinuation();

		    _outputWriter.Write(MimeType.Json, JsonUtil.ToJson(continuation.ToDictionary()));

		    return FubuContinuation.EndWithStatusCode(HttpStatusCode.Unauthorized);
		}

	    public AjaxContinuation BuildAjaxContinuation()
	    {
	        var url = _urls.UrlFor(new LoginRequest(), "GET");
	        var continuation = new AjaxContinuation {Success = false, NavigatePage = url};
	        return continuation;
	    }
	}
}