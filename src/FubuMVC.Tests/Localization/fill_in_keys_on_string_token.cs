using FubuMVC.Core.Localization;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Localization
{
    
    public class fill_in_keys_on_string_token
    {
        [Fact]
        public void all_keys_are_derived_from_the_field_name()
        {
            MyKeys.Key1.ToLocalizationKey().Key1.ShouldBe("MyKeys:Key1");

            MyKeys.Key1.Key.ShouldBe("Key1");
            MyKeys.Key2.Key.ShouldBe("Key2");
            MyKeys.Key3.Key.ShouldBe("Key3");

            MyKeys.Key1.DefaultValue.ShouldBe("Default1");
        }

        [Fact]
        public void all_keys_are_derived_from_the_field_name_2()
        {
            MyKeys2.Key1.ToLocalizationKey().Key1.ShouldBe("MyKeys2:Key1");

            MyKeys2.Key1.Key.ShouldBe("Key1");
            MyKeys2.Key2.Key.ShouldBe("Key2");
            MyKeys2.Key3.Key.ShouldBe("Key3");

            MyKeys2.Key1.DefaultValue.ShouldBe("Default1");
        }
    }

    public class MyKeys : StringToken
    {
        public static readonly MyKeys Key1 = new MyKeys("Default1");
        public static readonly MyKeys Key2 = new MyKeys("Default2");
        public static readonly MyKeys Key3 = new MyKeys("Default3");

        public MyKeys(string defaultValue) : base(null, defaultValue, "MyKeys")
        {
        }
    }

    public class MyKeys2 : StringToken
    {
        public static readonly MyKeys2 Key1 = new MyKeys2("Default1");
        public static readonly MyKeys2 Key2 = new MyKeys2("Default2");
        public static readonly MyKeys2 Key3 = new MyKeys2("Default3");

        public MyKeys2(string defaultValue)
            : base(null, defaultValue, namespaceByType:true)
        {
        }
    }
}