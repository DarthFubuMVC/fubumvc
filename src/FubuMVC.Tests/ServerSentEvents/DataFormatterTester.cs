using FubuMVC.Core.ServerSentEvents;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServerSentEvents
{
	[TestFixture]
	public class DataFormatterTester
	{
		private DataFormatter theFormatter;
		private string theOutput;

		[TestFixtureSetUp]
		public void SetUp()
		{
			theFormatter = new DataFormatter();
			theOutput = theFormatter.DataFor(new Output("test"));
		}

		[Test]
		public void just_uses_the_json_util_to_serialize_the_data()
		{
			theOutput.ShouldBe("{\"name\":\"test\"}");
		}

		public class Output
		{
			public Output(string name)
			{
				this.name = name;
			
			}

			public string name { get; set; }
		}
	}
}