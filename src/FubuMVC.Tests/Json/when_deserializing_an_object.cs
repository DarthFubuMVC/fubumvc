using System;
using FubuCore;
using FubuCore.Conversion;
using FubuMVC.Core.Json;
using Shouldly;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace FubuMVC.Tests.Json
{

    [TestFixture]
     public class respects_the_injected_settings_in_serialization
     {
        [Test]
        public void round_trip_with_camel_casing()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var serializer = new NewtonSoftJsonSerializer(settings, new JsonConverter[0]);
            var json = serializer.Serialize(new Target {Name = "Jeremy"});
            json.ShouldBe("{\"name\":\"Jeremy\"}");

            var target2 = serializer.Deserialize<Target>(json);
            target2.Name.ShouldBe("Jeremy");
        }    
     }

    public class Target
    {
        public string Name { get; set; }
    }


	[TestFixture]
	public class when_deserializing_an_object
	{
		private NewtonSoftJsonSerializer theSerializer;
		private string theInput;
		private ParentType theObject;

		[SetUp]
		public void SetUp()
		{
			var locator = new InMemoryServiceLocator();
			var objectConverter = new ObjectConverter(locator, new ConverterLibrary(new[] {new StatelessComplexTypeConverter()}));
			locator.Add<IObjectConverter>(objectConverter);

			var converter = new ComplexTypeConverter(objectConverter);

			theInput = "{\"Name\":\"Test\",\"Child\":\"x:123\"}";
			theSerializer = new NewtonSoftJsonSerializer(new JsonSerializerSettings(), new[] { converter });

			theObject = theSerializer.Deserialize<ParentType>(theInput);
		}

		[Test]
		public void uses_the_object_converter()
		{
			theObject.Name.ShouldBe("Test");
			theObject.Child.ShouldBe(new ComplexType {Key = "x", Value = "123"});
		}
	}

	public class StatelessComplexTypeConverter : StatelessConverter<ComplexType>
	{
		protected override ComplexType convert(string text)
		{
			var values = text.Split(new[] { ":" }, StringSplitOptions.None);
			return new ComplexType {Key = values[0], Value = values[1]};
		}
	}
}