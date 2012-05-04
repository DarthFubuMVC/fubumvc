using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class InputNodeTester
    {
        [Test]
        public void IMayHaveInputType_implementation()
        {
            var node = new InputNode(typeof(Address));
            node.As<IMayHaveInputType>().InputType().ShouldEqual(typeof (Address));
        }

        [Test]
        public void ClearAll()
        {
            var node = new InputNode(typeof (Address));
            node.AddFormatter<JsonFormatter>();
            node.AllowHttpFormPosts = true;

            node.ClearAll();

            node.Readers.Any().ShouldBeFalse();
            node.AllowHttpFormPosts.ShouldBeFalse();
        }

        [Test]
        public void JsonOnly_from_scratch()
        {
            var node = new InputNode(typeof(Address));
            node.JsonOnly();

            node.Readers.ShouldHaveCount(1);
            node.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void JsonOnly_with_existing_stuff()
        {
            var node = new InputNode(typeof(Address));
            node.AllowHttpFormPosts = true;
            node.UsesFormatter<XmlFormatter>();

            node.JsonOnly();

            node.Readers.ShouldHaveCount(1);
            node.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void add_formatter()
        {
            var node = new InputNode(typeof (Address));
            node.AllowHttpFormPosts = false;
            
            node.AddFormatter<JsonFormatter>();

            node.Readers.Single()
                .ShouldEqual(new ReadWithFormatter(typeof (Address), typeof (JsonFormatter)));
        }

        [Test]
        public void add_formatter_is_idempotent()
        {
            var node = new InputNode(typeof (Address));
            node.AllowHttpFormPosts = false;

            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<JsonFormatter>();
            node.AddFormatter<JsonFormatter>();

            node.Readers.Single()
                .ShouldEqual(new ReadWithFormatter(typeof (Address), typeof (JsonFormatter)));
        }


        [Test]
        public void add_reader_happy_path()
        {
            var node = new InputNode(typeof (Address));
            node.AllowHttpFormPosts = false;

            var reader = node.AddReader<FakeAddressReader>();

            node.Readers.Single().ShouldBeTheSameAs(reader);

            reader.InputType.ShouldEqual(typeof (Address));
            reader.ReaderType.ShouldEqual(typeof (FakeAddressReader));
        }

        [Test]
        public void allow_http_form_post_is_idempotent()
        {
            var inputNode = new InputNode(typeof (Address));
            inputNode.AllowHttpFormPosts = true;
            inputNode.AllowHttpFormPosts = true;
            inputNode.AllowHttpFormPosts = true;

            inputNode.Readers.Single().ShouldBeOfType<ModelBind>();
        }

        [Test]
        public void allow_http_form_posts_adds_the_model_bind_reader()
        {
            var inputNode = new InputNode(typeof (Address));
            inputNode.AllowHttpFormPosts = false;

            inputNode.Readers.Any().ShouldBeFalse();

            inputNode.AllowHttpFormPosts = true;

            inputNode.Readers.Single().ShouldBeOfType<ModelBind>()
                .InputType.ShouldEqual(typeof (Address));
        }

        [Test]
        public void setting_allow_http_form_post_to_false_removes_the_model_binding()
        {
            var inputNode = new InputNode(typeof (Address));
            inputNode.AllowHttpFormPosts = true;

            inputNode.AllowHttpFormPosts = false;

            inputNode.Readers.Any().ShouldBeFalse();
        }

        [Test]
        public void should_allow_form_posts_by_default()
        {
            var inputNode = new InputNode(typeof (Address));
            inputNode.AllowHttpFormPosts
                .ShouldBeTrue();
        }

        [Test]
        public void uses_formatter()
        {
            var node = new InputNode(typeof(Address));
        
            node.UsesFormatter<XmlFormatter>().ShouldBeFalse();
            node.UsesFormatter<JsonFormatter>().ShouldBeFalse();

            node.AddFormatter<XmlFormatter>();

            node.UsesFormatter<XmlFormatter>().ShouldBeTrue();
            node.UsesFormatter<JsonFormatter>().ShouldBeFalse();

            node.AddFormatter<JsonFormatter>();

            node.UsesFormatter<XmlFormatter>().ShouldBeTrue();
            node.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }
    }


    public class InputTarget
    {
    }

    [MimeType("text/xml", "text/json")]
    public class SpecificReader : IReader<InputTarget>
    {
        public IEnumerable<string> Mimetypes
        {
            get { throw new NotImplementedException(); }
        }

        public InputTarget Read(string mimeType)
        {
            throw new NotImplementedException();
        }

        public void Write(string mimeType, OutputTarget resource)
        {
            throw new NotImplementedException();
        }
    }

    public class GenericReader<T> : IReader<T>
    {
        public IEnumerable<string> Mimetypes
        {
            get { throw new NotImplementedException(); }
        }

        public T Read(string mimeType)
        {
            throw new NotImplementedException();
        }

        public void Write(string mimeType, T resource)
        {
            throw new NotImplementedException();
        }
    }

    public class FancyReader<T> : IReader<T>
    {
        public T Read(string mimeType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return "fancy/Reader"; }
        }
    }

    public class FakeAddressReader : IReader<Address>
    {
        public IEnumerable<string> Mimetypes
        {
            get { yield return "fake/address"; }
        }

        public Address Read(string mimeType)
        {
            throw new NotImplementedException();
        }
    }
}