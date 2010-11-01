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
            return new InputParser().BuildHandler(property);
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
            handlerFor(x => x.ColorFlag).ShouldBeOfType<EnumerationFlag>();
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
    }
}