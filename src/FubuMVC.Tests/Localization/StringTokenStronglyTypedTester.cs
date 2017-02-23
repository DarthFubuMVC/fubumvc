using FubuMVC.Core.Localization;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Localization
{
    
    public class StringTokenStronglyTypedTester
    {
        [Fact]
        public void simple_localization()
        {
            var token = Loc.Basic;
            var locKey = token.ToLocalizationKey();
            locKey.Key1.ShouldBe("Loc:Basic");
            token.DefaultValue.ShouldBe("Basic_Trans");
        }

        [Fact]
        public void nested_class_localization()
        {
            var token = Loc.Account.AccountName;
            var locKey = token.ToLocalizationKey();

            locKey.Key1.ShouldBe("Loc.Account:AccountName");
            token.DefaultValue.ShouldBe("Account_Trans");
        }

        [Fact]
        public void parameter_localization()
        {
            var token = Loc.User.NameWithParams;
            var locKey = token.ToLocalizationKey();

            locKey.Key1.ShouldBe("Loc.User:NameWithParams");
            token.FormatTokenWith(new NameParams { LastName = "Sir" }).ShouldBe("Name_Sir");
        }

        [Fact]
        public void parameter_localization_raw_key()
        {
            var token = Loc.User.NameWithParams;
            token.ToRawString().ShouldBe("Name_{LastName}");
        }
    }

    public partial class Loc
    {
        public static readonly StringToken Basic = new StringToken<Loc>("Basic_Trans");

        public class User
        {
            public static readonly StringToken Name = new StringToken<User>("Name_Trans");

            public static readonly StringToken<User, NameParams> NameWithParams =
                new StringToken<User, NameParams>("Name_{LastName}");
        }
    }

    public class NameParams
    {
        public string LastName { get; set; }
    }

    public partial class Loc
    {
        public class Account
        {
            public static readonly StringToken AccountName = new StringToken<Account>("Account_Trans");
        }
    }
}