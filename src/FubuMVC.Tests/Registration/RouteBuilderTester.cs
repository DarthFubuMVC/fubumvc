using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Routes;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Registration
{
    
    public class when_creating_the_route_for_an_input_from_a_good_pattern_with_no_defaults
    {
        #region Setup/Teardown

        public when_creating_the_route_for_an_input_from_a_good_pattern_with_no_defaults()
        {
            thePattern = "here/there/{Name}/{Age}";
            route = RouteBuilder.Build<InputModel>(thePattern);
        }

        #endregion

        private IRouteDefinition route;
        private string thePattern;

        [Fact]
        public void has_a_route_input_for_each_property_in_the_pattern()
        {
            route.Input.RouteParameters.Select(x => x.Name).ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Fact]
        public void set_the_pattern_correctly()
        {
            route.Pattern.ShouldBe(thePattern);
        }

        [Fact]
        public void rank()
        {
            RouteBuilder.PatternRank("{foo}/").ShouldBe(1);
            RouteBuilder.PatternRank("foo").ShouldBe(0);
            RouteBuilder.PatternRank("{foo}/to/{bar}").ShouldBe(2);
        }
    }

    
    public class when_creating_a_route_for_an_input_from_a_pattern_with_invalid_property_name
    {
        [Fact]
        public void should_throw_a_fubu_exception()
        {
            const string thePattern = "here/there/{Name}/{DoesNotExist}";

            Exception<FubuException>.ShouldBeThrownBy(() => RouteBuilder.Build<InputModel>(thePattern));
        }
    }


    
    public class when_creating_the_route_for_an_input_from_a_good_pattern_with_no_defaults_from_the_type
    {
        public when_creating_the_route_for_an_input_from_a_good_pattern_with_no_defaults_from_the_type()
        {
            thePattern = "here/there/{Name}/{Age}";
            route = RouteBuilder.Build(typeof (InputModel), thePattern);
        }


        private IRouteDefinition route;
        private string thePattern;

        [Fact]
        public void has_a_route_input_for_each_property_in_the_pattern()
        {
            route.Input.RouteParameters.Select(x => x.Name).ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Fact]
        public void set_the_pattern_correctly()
        {
            route.Pattern.ShouldBe(thePattern);
        }
    }

    
    public class when_creating_the_route_for_an_input_from_a_good_pattern_with_defaults
    {
        public when_creating_the_route_for_an_input_from_a_good_pattern_with_defaults()
        {
            thePattern = "here/there/{Name:Josh}/{Age}";
            route = RouteBuilder.Build<InputModel>(thePattern);
        }


        private string thePattern;
        private IRouteDefinition route;

        [Fact]
        public void pick_up_the_default_value_if_it_exists()
        {
            route.Input.RouteParameters.First(x => x.Name == "Name").DefaultValue.ShouldBe("Josh");
        }

        [Fact]
        public void should_have_a_null_default_for_input_that_has_no_default_in_the_pattern()
        {
            route.Input.RouteParameters.First(x => x.Name == "Age").DefaultValue.ShouldBeNull();
        }
    }

    public class InputModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Age2 { get; set; }
    }

    public class InputModelWithEquals
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public bool Equals(InputModelWithEquals other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && other.Age == Age;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (InputModelWithEquals)) return false;
            return Equals((InputModelWithEquals) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ Age;
            }
        }

        public static bool operator ==(InputModelWithEquals left, InputModelWithEquals right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InputModelWithEquals left, InputModelWithEquals right)
        {
            return !Equals(left, right);
        }
    }
}