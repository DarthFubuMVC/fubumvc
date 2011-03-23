using Newtonsoft.Json;

namespace FubuMVC.Diagnostics.Infrastructure
{
	public interface IJsonProvider
	{
		T Deserialize<T>(string input);
	}

	public class JsonProvider : IJsonProvider
	{
		public T Deserialize<T>(string input)
		{
			return JsonConvert.DeserializeObject<T>(input);
		}
	}

	public class JsonService
	{
		static JsonService()
		{
			Stub(new JsonProvider());
		}

		public static IJsonProvider Provider { get; private set; }

		public static void Stub(IJsonProvider provider)
		{
			Provider = provider;
		}

		public static T Deserialize<T>(string input)
		{
			return Provider.Deserialize<T>(input);
		}
	}
}