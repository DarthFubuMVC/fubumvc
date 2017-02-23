﻿using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Registration;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Registration
{
    
    public class FuncBuilderTester
    {
        [Fact]
        public void can_compile_getter()
        {
            var sample = new SampleClass {Message = "Success!"};

            var func = FuncBuilder.CompileGetter(sample.GetType().GetProperty("Message")).As<Func<SampleClass, string>>();
            var result = func(sample);
            result.ShouldBe(sample.Message);
        }

        [Fact]
        public void can_compile_setter()
        {
            var message = "Success!";
            var sample = new SampleClass {Message = "Failed!"};

            var func = FuncBuilder.CompileSetter(sample.GetType().GetProperty("Message")).As<Action<SampleClass, string>>();
            func(sample, message);
            sample.Message.ShouldBe(message);
        }
    }

    public class SampleClass
    {
        public string Message { get; set; }
    }
}