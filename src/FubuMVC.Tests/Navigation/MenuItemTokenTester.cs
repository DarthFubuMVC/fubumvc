using System;
using FubuMVC.Core.Navigation;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
	[TestFixture]
	public class MenuItemTokenTester
	{
		[Test]
		public void get_data()
		{
			var token = new MenuItemToken();
			var value = Guid.NewGuid();

			token.Data.Add("Test", value);

			token.Get<Guid>("Test").ShouldBe(value);
		}

		[Test]
		public void has_data()
		{
			var token = new MenuItemToken();
			var value = Guid.NewGuid();

			token.Data.Add("Test", value);

			token.Has("Test").ShouldBeTrue();
		}

		[Test]
		public void value_continuation_for_existing_value()
		{
			var token = new MenuItemToken();
			var expected = Guid.NewGuid();

			token.Set("key", expected);

			token.Value<Guid>("key", x => x.ShouldBe(expected));
		}

		[Test]
		public void value_continuation_for_non_existing_value()
		{
			var token = new MenuItemToken();
			var invoked = false;

			token.Value<Guid>("key", x => invoked = true);

			invoked.ShouldBeFalse();
		}
	}
}