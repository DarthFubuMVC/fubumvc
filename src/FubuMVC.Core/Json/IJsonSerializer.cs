using System.IO;

namespace FubuMVC.Core.Json
{
	public interface IJsonSerializer
	{
		string Serialize(object target, bool includeMetadata = false);
		T Deserialize<T>(string input);
	    T Deserialize<T>(Stream stream);
	}
}