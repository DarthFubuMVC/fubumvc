using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Projections
{
    public class JsonSerializerTester
    {
        private JsonSerializer ClassUnderTest;
        private IFubuRequestContext theFubuRequestContext;
        private IHttpRequest theHttpRequest;
        private InMemoryOutputWriter theOutputWriter;

        private JsonTestData GenerateJsonTestData(int numberOfGuids)
        {
            var guids = new List<Guid>();
            for (var i = 0; i < numberOfGuids; i++)
            {
                guids.Add(Guid.NewGuid());
            }
            return new JsonTestData
            {
                Guids = guids
            };
        }

        [SetUp]
        public void SetUp()
        {
            ClassUnderTest = new JsonSerializer();
            theFubuRequestContext = MockRepository.GenerateMock<IFubuRequestContext>();
            theHttpRequest = MockRepository.GenerateMock<IHttpRequest>();
            theOutputWriter = new InMemoryOutputWriter();
            theFubuRequestContext.Stub(x => x.Request).Return(theHttpRequest);
            theFubuRequestContext.Stub(x => x.Writer).Return(theOutputWriter);
        }

        [Test]
        public void writing_large_object()
        {
            var data = GenerateJsonTestData(60000);
            ClassUnderTest.Write(theFubuRequestContext, data, "application/json");
            theOutputWriter.ToString().ShouldNotBeEmpty();
        }

        [Test]
        public void writing_small_object()
        {
            var data = GenerateJsonTestData(10);
            ClassUnderTest.Write(theFubuRequestContext, data, "application/json");
            theOutputWriter.ToString().ShouldNotBeEmpty();
        }

        [Test]
        public void reading_large_object()
        {
            var data = GenerateJsonTestData(60000);
            var json = HtmlTags.JsonUtil.ToJson(data);
            var bytes = Encoding.Default.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            {
                theHttpRequest.Stub(x => x.Input).Return(stream);
                ClassUnderTest.Read<JsonTestData>(theFubuRequestContext).Guids.ShouldHaveCount(60000);
            }
        }

        [Test]
        public void reading_small_object()
        {
            var data = GenerateJsonTestData(10);
            var json = HtmlTags.JsonUtil.ToJson(data);
            var bytes = Encoding.Default.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            {
                theHttpRequest.Stub(x => x.Input).Return(stream);
                ClassUnderTest.Read<JsonTestData>(theFubuRequestContext).Guids.ShouldHaveCount(10);
            }
        }
    }

    public class JsonTestData
    {
        public List<Guid> Guids { get; set; }
    }
}