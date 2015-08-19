using HtmlTags;

namespace FubuMVC.Core.ServerSentEvents
{
	public interface IDataFormatter
	{
		string DataFor(object value);
	}

	public class DataFormatter : IDataFormatter
	{
		public string DataFor(object value)
		{
			return JsonUtil.ToJson(value);
		}
	}
}