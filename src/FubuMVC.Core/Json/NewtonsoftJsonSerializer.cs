using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace FubuMVC.Core.Json
{


	public class NewtonSoftJsonSerializer : IJsonSerializer
	{
	    private readonly JsonSerializerSettings _settings;
	    private readonly IEnumerable<JsonConverter> _converters;

		public NewtonSoftJsonSerializer(JsonSerializerSettings settings, IEnumerable<JsonConverter> converters)
		{
		    _settings = settings;
		    _converters = converters;
		}

	    public JsonSerializer InnerSerializer()
	    {
	        return buildSerializer(false);
	    }

	    private JsonSerializer buildSerializer(bool includeMetadata)
	    {
	        var jsonSerializer = JsonSerializer.Create(_settings);

            if (includeMetadata)
            {
                jsonSerializer.TypeNameHandling = TypeNameHandling.All;
            }
	        
			jsonSerializer.Converters.AddRange(_converters);
			
			return jsonSerializer;
		}

		public string Serialize(object target, bool includeMetadata = false)
		{
			var stringWriter = new StringWriter();
			var writer = new JsonTextWriter(stringWriter);

			var serializer = buildSerializer(includeMetadata);
			serializer.Serialize(writer, target);

			return stringWriter.ToString();
		}

		public T Deserialize<T>(string input)
		{
			var serializer = buildSerializer(true);
			return serializer.Deserialize<T>(new JsonTextReader(new StringReader(input)));
		}

	    public T Deserialize<T>(Stream stream)
	    {
	        return buildSerializer(false).Deserialize<T>(new JsonTextReader(new StreamReader(stream)));
	    }
	}
}