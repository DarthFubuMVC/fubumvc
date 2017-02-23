using System;
using FubuMVC.Core.Navigation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
	
	public class MenuItemTokenTester
	{
		[Fact]
		public void get_data()
		{
			var token = new MenuItemToken();
			var value = Guid.NewGuid();

			token.Data.Add("Test", value);

			token.Get<Guid>("Test").ShouldBe(value);
		}

		[Fact]
		public void has_data()
		{
			var token = new MenuItemToken();
			var value = Guid.NewGuid();

			token.Data.Add("Test", value);

			token.Has("Test").ShouldBeTrue();
		}

		[Fact]
		public void value_continuation_for_existing_value()
		{
			var token = new MenuItemToken();
			var expected = Guid.NewGuid();

			token.Set("key", expected);

			token.Value<Guid>("key", x => x.ShouldBe(expected));
		}

		[Fact]
		public void value_continuation_for_non_existing_value()
		{
			var token = new MenuItemToken();
			var invoked = false;

			token.Value<Guid>("key", x => invoked = true);

			invoked.ShouldBeFalse();
		}
	}
}