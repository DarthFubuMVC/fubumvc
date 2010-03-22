using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class DeserializeJsonBehaviorTester
    {
        private Message message;
        private InMemoryFubuRequest request;
        private InMemoryStreamingData data;

        [SetUp]
        public void SetUp()
        {
            request = new InMemoryFubuRequest();

            message = new Message
            {
                Number = 10,
                Details = new Detail[]
                {
                    new Detail("Jeremy", 36),
                    new Detail("Max", 6),
                    new Detail("Natalie", 28),
                }
            };

            data = new InMemoryStreamingData();
            data.JsonInputIs(message);
        }

        private void executeBehavior(IJsonReader reader)
        {
            var behavior = new DeserializeJsonBehavior<Message>(reader, request);
            var next = MockRepository.GenerateMock<IActionBehavior>();
            behavior.InsideBehavior = next;

            behavior.Invoke();

            next.AssertWasCalled(x => x.Invoke());
        }

        
        public void assertThatTheMessageIsStoredIntoFubuRequest()
        {
            var inputMessage = request.Get<Message>();
            inputMessage.ShouldNotBeNull();
            inputMessage.Number.ShouldEqual(message.Number);

            inputMessage.ShouldNotBeTheSameAs(message);

            inputMessage.Details.ShouldHaveTheSameElementsAs(message.Details);
        }

        [Test]
        public void deserialize_with_data_contract_json_serializer()
        {
            var serializer = new DataContractJsonSerializer(typeof (Message));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, message);
            stream.Position = 0;

            string json = new StreamReader(stream).ReadToEnd();
            data.JsonInputIs(json);

            var reader = new DataContractJsonReader(data);
            executeBehavior(reader);

            assertThatTheMessageIsStoredIntoFubuRequest();
        }

        [Test]
        public void deserialize_with_javascript_serializer()
        {
            var reader = new JavaScriptJsonReader(data);
            executeBehavior(reader);

            assertThatTheMessageIsStoredIntoFubuRequest();
        }
    }

    [DataContract]
    public class Detail
    {
        public Detail()
        {
        }

        public Detail(string name, int age)
        {
            Name = name;
            Age = age;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Age { get; set; }

        public bool Equals(Detail other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && other.Age == Age;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Detail)) return false;
            return Equals((Detail) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ Age;
            }
        }
    }

    [Serializable]
    public class Message
    {
        public Detail[] Details { get; set; }

        public int Number { get; set; }
    }
}