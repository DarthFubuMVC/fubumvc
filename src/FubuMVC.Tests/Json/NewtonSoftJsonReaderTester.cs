using FubuMVC.Core.Json;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Json
{
	[TestFixture]
	public class NewtonSoftJsonReaderTester : InteractionContext<NewtonSoftJsonReader>
	{
		private string theJson;
		private ParentType theTarget;

		protected override void beforeEach()
		{
			theJson = "{something}";

			Services.PartialMockTheClassUnderTest();
			ClassUnderTest.Stub(x => x.GetInputText()).Return(theJson);

			theTarget = new ParentType { Name = "Blah", Child = new ComplexType { Value = "123", Key = "x"}};

			MockFor<IJsonSerializer>().Expect(x => x.Deserialize<ParentType>(theJson)).Return(theTarget);
		}

		[Test]
		public void just_delegates_to_the_json_serializer()
		{
			ClassUnderTest.Read<ParentType>().ShouldBe(theTarget);
		}
	}
}