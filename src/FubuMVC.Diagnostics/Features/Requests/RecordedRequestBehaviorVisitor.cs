using System.Data;
using System.Net;
using System.Text;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;

namespace FubuMVC.Diagnostics.Features.Requests
{
	public class RecordedRequestBehaviorVisitor : IBehaviorDetailsVisitor
	{
		private readonly StringBuilder _exceptionBuilder = new StringBuilder();
		private HttpStatusCode _statusCode = HttpStatusCode.OK;

		public HttpStatusCode StatusCode
		{
			get { return _statusCode; }
		}

		public void ModelBinding(ModelBindingReport report)
		{
		}

		public void FileOutput(FileOutputReport report)
		{
		}

		public void WriteOutput(OutputReport report)
		{
		}

		public void Redirect(RedirectReport report)
		{
		}

		public void Exception(ExceptionReport report)
		{
			if(_exceptionBuilder.Length != 0)
			{
				_exceptionBuilder.Append("; ");
			}

			_exceptionBuilder.Append(report.Text);
		}

		public void SetValue(SetValueReport report)
		{
		}

		public void Authorization(AuthorizationReport report)
		{
		}

		public void CustomTable(DataTable report)
		{
		}

		public void HttpStatus(HttpStatusReport report)
		{
			_statusCode = report.Status;
		}

		public bool HasExceptions()
		{
			return _exceptionBuilder.Length != 0;
		}

		public string Exceptions()
		{
			return _exceptionBuilder.ToString();
		}
	}
}