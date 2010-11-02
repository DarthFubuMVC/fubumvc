using System;
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
        
    }
}