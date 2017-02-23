using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Json;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Resources.Conneg
{
    
    public class InputNodeTester
    {
        [Fact]
        public void IMayHaveInputType_implementation()
        {
            var node = new InputNode(typeof (Address));
            node.As<IMayHaveInputType>().InputType().ShouldBe(typeof (Address));
        }

        [Fact]
        public void ClearAll()
        {
            var node = new InputNode(typeof (Address));
            node.Add(new NewtonsoftJsonFormatter());
            node.Add(typeof (ModelBindingReader<>));

            node.ClearAll();

            node.Explicits.Any().ShouldBeFalse();
        }


        [Fact]
        public void add_reader_by_formatter()
        {
            var node = new InputNode(typeof (Address));
            var formatter = new NewtonsoftJsonFormatter();
            node.Add(formatter);

            node.Explicits.Single()
                .ShouldBeOfType<FormatterReader<Address>>()
                .Formatter.ShouldBeTheSameAs(formatter);
        }

        [Fact]
        public void add_reader_by_type()
        {
            var node = new InputNode(typeof (Address));
            node.Add(typeof (GenericReader<>));

            node.Explicits.Single()
                .ShouldBeOfType<GenericReader<Address>>();
        }

        [Fact]
        public void add_reader_by_type_sad_path()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new InputNode(typeof (Address))
                    .Add(GetType());
            });
        }

        [Fact]
        public void add_reader_by_instance_happy_path()
        {
            var node = new InputNode(typeof (InputTarget));
            var reader = new SpecificReader();

            node.Add(reader);

            node.Explicits.Single().ShouldBeTheSameAs(reader);
        }

        [Fact]
        public void add_reader_by_instance_sad_path()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new InputNode(typeof (Address))
                    .Add(new SpecificReader());
            });
        }
    }


    public class InputTarget
    {
    }

    public class SpecificReader : IReader<InputTarget>
    {
        public IEnumerable<string> Mimetypes
        {
            get { throw new NotImplementedException(); }
        }

        public InputTarget Read(string mimeType, IFubuRequestContext context)
        {
            throw new NotImplementedException();
        }

        public void Write(string mimeType, OutputTarget resource)
        {
            throw new NotImplementedException();
        }

        public Type ModelType
        {
            get { return typeof (InputTarget); }
        }
    }

    public class GenericReader<T> : IReader<T>
    {
        public IEnumerable<string> Mimetypes
        {
            get { throw new NotImplementedException(); }
        }

        public T Read(string mimeType, IFubuRequestContext context)
        {
            throw new NotImplementedException();
        }

        public void Write(string mimeType, T resource)
        {
            throw new NotImplementedException();
        }

        public Type ModelType
        {
            get { return typeof (T); }
        }
    }

    public class FancyReader<T> : IReader<T>
    {
        public T Read(string mimeType, IFubuRequestContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return "fancy/Reader"; }
        }

        public Type ModelType
        {
            get { return typeof (T); }
        }
    }

    public class FakeAddressReader : IReader<Address>
    {
        public IEnumerable<string> Mimetypes
        {
            get { yield return "fake/address"; }
        }

        public Address Read(string mimeType, IFubuRequestContext context)
        {
            throw new NotImplementedException();
        }

        public Type ModelType
        {
            get { return typeof (Address); }
        }
    }
}