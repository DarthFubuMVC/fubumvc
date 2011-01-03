using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.CommandLine;
using NUnit.Framework;
using FubuCore.Reflection;


namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class InputParserTester
    {
        private InputModel theInput;

        [SetUp]
        public void SetUp()
        {
            theInput = new InputModel();
        }


        private ITokenHandler handlerFor(Expression<Func<InputModel, object>> expression)
        {
            var property = expression.ToAccessor().InnerProperty;
            return InputParser.BuildHandler(property);
        }

        private bool handle(Expression<Func<InputModel, object>> expression, params string[] args)
        {
            var queue = new Queue<string>(args);

            var handler = handlerFor(expression);

            return handler.Handle(theInput, queue);
        }

        [Test]
        public void the_handler_for_a_normal_property_not_marked_as_flag()
        {
            handlerFor(x => x.File).ShouldBeOfType<Argument>();
        }

        [Test]
        public void the_handler_for_an_enumeration_property_marked_as_flag()
        {
            handlerFor(x => x.ColorFlag).ShouldBeOfType<Flag>();
        }

        [Test]
        public void the_handler_for_an_enumeration_property_not_marked_as_flag()
        {
            handlerFor(x => x.Color).ShouldBeOfType<Argument>();
        }

        [Test]
        public void the_handler_for_a_property_suffixed_by_flag()
        {
            handlerFor(x => x.OrderFlag).ShouldBeOfType<Flag>();
        }

        [Test]
        public void the_handler_for_a_boolean_flag()
        {
            handlerFor(x => x.TrueFalseFlag).ShouldBeOfType<BooleanFlag>();
        }

        [Test]
        public void get_the_flag_name_for_a_property()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.OrderFlag);
            InputParser.ToFlagName(property).ShouldEqual("-order");
        }


        [Test]
        public void get_the_flag_name_for_a_property_with_an_alias()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.AliasedFlag);
            InputParser.ToFlagName(property).ShouldEqual("-a");
        }

        [Test]
        public void boolean_flag_does_not_catch()
        {
            handle(x => x.TrueFalseFlag, "nottherightthing").ShouldBeFalse();
            theInput.TrueFalseFlag.ShouldBeFalse();
        }

        [Test]
        public void boolean_flag_does_catch()
        {
            handle(x => x.TrueFalseFlag, "-TrueFalse").ShouldBeTrue();
            theInput.TrueFalseFlag.ShouldBeTrue();
        }


        [Test]
        public void boolean_flag_does_catch_2()
        {
            handle(x => x.TrueFalseFlag, "-truefalse").ShouldBeTrue();
            theInput.TrueFalseFlag.ShouldBeTrue();
        }

        [Test]
        public void boolean_flag_does_catch_case_insensitive()
        {
            handle(x => x.TrueFalseFlag, "-trueFalse").ShouldBeTrue();
            theInput.TrueFalseFlag.ShouldBeTrue();
        }

        [Test]
        public void enumeration_argument()
        {
            handle(x => x.Color, "red").ShouldBeTrue();
            theInput.Color.ShouldEqual(Color.red);
        }

        [Test]
        public void enumeration_argument_2()
        {
            handle(x => x.Color, "green").ShouldBeTrue();
            theInput.Color.ShouldEqual(Color.green);
        }

        [Test]
        public void enumeration_flag_negative()
        {
            handle(x => x.ColorFlag, "green").ShouldBeFalse();
        }

        [Test]
        public void enumeration_flag_positive()
        {
            handle(x => x.ColorFlag, "-color", "blue").ShouldBeTrue();
            theInput.ColorFlag.ShouldEqual(Color.blue);
        }


        [Test]
        public void string_argument()
        {
            handle(x => x.File, "the file").ShouldBeTrue();
            theInput.File.ShouldEqual("the file");
        }

        [Test]
        public void int_flag_does_not_catch()
        {
            handle(x => x.OrderFlag, "not order flag").ShouldBeFalse();
            theInput.OrderFlag.ShouldEqual(0);
        }

        [Test]
        public void int_flag_catches()
        {
            handle(x => x.OrderFlag, "-order", "23").ShouldBeTrue();
            theInput.OrderFlag.ShouldEqual(23);
        }

        private InputModel build(params string[] tokens)
        {
            var queue = new Queue<string>(tokens);
            var graph = new UsageGraph(typeof (InputCommand));

            return (InputModel) graph.BuildInput(queue);
        }

        [Test]
        public void integrated_test_arguments_only()
        {
            var input = build("file1", "red");
            input.File.ShouldEqual("file1");
            input.Color.ShouldEqual(Color.red);

            // default is not touched
            input.OrderFlag.ShouldEqual(0);
        }

        [Test]
        public void integrated_test_with_mix_of_flags()
        {
            var input = build("file1", "-color", "green", "blue", "-order", "12");
            input.File.ShouldEqual("file1");
            input.Color.ShouldEqual(Color.blue);
            input.ColorFlag.ShouldEqual(Color.green);
            input.OrderFlag.ShouldEqual(12);
        }

        [Test]
        public void integrated_test_with_a_boolean_flag()
        {
            var input = build("file1", "blue", "-truefalse");
            input.TrueFalseFlag.ShouldBeTrue();

            build("file1", "blue").TrueFalseFlag.ShouldBeFalse();
        }
    }


    public enum Color
    {
        red,
        green,
        blue
    }

    public class InputModel
    {
        public string File { get; set; }
        public Color ColorFlag { get; set; }
        public Color Color { get; set; }
        public int OrderFlag { get; set; }
        public bool TrueFalseFlag { get; set; }

        [FlagAlias("a")]
        public string AliasedFlag { get; set; }
    }

    public class InputCommand : FubuCommand<InputModel>
    {
        public override void Execute(InputModel input)
        {
            throw new NotImplementedException();
        }
    }
}