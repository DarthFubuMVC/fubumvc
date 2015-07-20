using System;
using FubuCore.Conversion;
using Newtonsoft.Json;

namespace FubuMVC.Core.Json
{
	// Just a convenience helper
	// See the NewtonsoftJsonSerializer tests for examples
	public abstract class FubuJsonConverter<T> : JsonConverter
	{
		private readonly IObjectConverter _converter;

		public FubuJsonConverter(IObjectConverter converter)
		{
			_converter = converter;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			string jsonValue = toJsonValue((T)value);

			serializer.Serialize(writer, jsonValue);
		}

		protected abstract string toJsonValue(T value);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null) return default(T);

			return _converter.FromString<T>(reader.Value.ToString());
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.Equals(typeof(T));
		}
	}
}