using System;
using FubuCore;
using FubuTestingSupport;
using FubuTransportation.Registration;
using NUnit.Framework;

namespace FubuTransportation.Testing.Registration
{
    [TestFixture]
    public class FuncBuilderTester
    {
        [Test]
        public void can_compile_getter()
        {
            var sample = new SampleClass {Message = "Success!"};

            var func = FuncBuilder.CompileGetter(sample.GetType().GetProperty("Message")).As<Func<SampleClass, string>>();
            var result = func(sample);
            result.ShouldEqual(sample.Message);
        }

        [Test]
        public void can_compile_setter()
        {
            var message = "Success!";
            var sample = new SampleClass {Message = "Failed!"};

            var func = FuncBuilder.CompileSetter(sample.GetType().GetProperty("Message")).As<Action<SampleClass, string>>();
            func(sample, message);
            sample.Message.ShouldEqual(message);
        }
    }

    public class SampleClass
    {
        public string Message { get; set; }
    }
}