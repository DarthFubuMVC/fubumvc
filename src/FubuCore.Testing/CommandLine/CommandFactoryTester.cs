using System;
using System.Collections.Generic;
using FubuCore.CommandLine;
using NUnit.Framework;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class CommandFactoryTester
    {
        [Test]
        public void get_the_command_name_for_a_class_not_decorated_with_the_attribute()
        {
            CommandFactory.CommandNameFor(typeof (MyCommand)).ShouldEqual("my");
        }

        [Test]
        public void get_the_command_name_for_a_class_decorated_with_the_attribute()
        {
            CommandFactory.CommandNameFor(typeof (DecoratedCommand)).ShouldEqual("this");
        }

        [Test]
        public void get_the_description_for_a_class_not_decorated_with_the_attribute()
        {
            CommandFactory.DescriptionFor(typeof (MyCommand)).ShouldEqual(typeof (MyCommand).FullName);
        }

        [Test]
        public void get_the_description_for_a_class_decorated_with_the_attribute()
        {
            CommandFactory.DescriptionFor(typeof (My2Command)).ShouldEqual("something");
        }

        [Test]
        public void get_the_command_name_for_a_class_decorated_with_the_attribute_but_without_the_name_specified()
        {
            CommandFactory.CommandNameFor(typeof(My2Command)).ShouldEqual("my2");
        }

        [Test]
        public void build()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            factory.Build("my").ShouldBeOfType<MyCommand>();
            factory.Build("my2").ShouldBeOfType<My2Command>();
            factory.Build("this").ShouldBeOfType<DecoratedCommand>();
        }

        [Test]
        public void build_command_from_a_string()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var run = factory.BuildRun("my Jeremy -force");

            run.Command.ShouldBeOfType<MyCommand>();
            var input = run.Input.ShouldBeOfType<MyCommandInput>();

            input.Name.ShouldEqual("Jeremy");
            input.ForceFlag.ShouldBeTrue();
        }

        [Test]
        public void fetch_the_help_command_run()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var run = factory.HelpRun();
            run.Command.ShouldBeTheSameAs(factory);
            run.Input.ShouldBeOfType<IEnumerable<Type>>()
                .ShouldContain(typeof(MyCommand));
        }

        [Test]
        public void fetch_the_help_command_if_the_args_are_empty()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var run = factory.BuildRun(new string[0]);
            run.Command.ShouldBeTheSameAs(factory);
            run.Input.ShouldBeOfType<IEnumerable<Type>>()
                .ShouldContain(typeof(MyCommand));
        }

        [Test]
        public void smoke_test_the_writing()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);
            factory.HelpRun().Execute();
        }


    }

    public class MyCommand : FubuCommand<MyCommandInput>
    {
        public override void Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }


    }

    [CommandDescription("something")]
    public class My2Command : FubuCommand<MyCommandInput>
    {
        public override void Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }
    }

    [CommandDescription("this", Name = "this")]
    public class DecoratedCommand : FubuCommand<MyCommandInput>
    {
        public override void Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }
    }


    public class MyCommandInput
    {
        public string Name { get; set; }
        public bool ForceFlag { get; set; }
    }
}