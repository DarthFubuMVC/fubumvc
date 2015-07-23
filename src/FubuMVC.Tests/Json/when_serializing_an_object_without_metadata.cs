using FubuCore.Conversion;
using FubuMVC.Core.Json;
using Shouldly;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FubuMVC.Tests.Json
{
	[TestFixture]
	public class when_serializing_an_object_without_metadata
	{
		private NewtonSoftJsonSerializer theSerializer;
		private ComplexTypeConverter theConverter;
		private ParentType theTarget;
		private string theResult;

		[SetUp]
		public void SetUp()
		{
			theConverter = new ComplexTypeConverter(new ObjectConverter());
			theSerializer = new NewtonSoftJsonSerializer(new JsonSerializerSettings(), new JsonConverter[] { theConverter });

			theTarget = new ParentType
			            	{
			            		Name = "Test",
								Child = new ComplexType { Key = "x", Value = "123" }
			            	};

			theResult = theSerializer.Serialize(theTarget);
		}

		[Test]
		public void uses_the_provided_converters()
		{
			theResult.ShouldBe("{\"Name\":\"Test\",\"Child\":\"x:123\"}");
		}
	}
}