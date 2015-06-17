using System;
using FubuTestingSupport;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Headers;
using FubuTransportation.Runtime.Serializers;
using NUnit.Framework;
using Rhino.Mocks;
using FubuCore;

namespace FubuTransportation.Testing.Runtime
{
    [TestFixture]
    public class EnvelopeTester
    {
        [Test]
        public void has_a_correlation_id_by_default()
        {
            new Envelope().CorrelationId.ShouldNotBeNull();

            new Envelope().CorrelationId.ShouldNotEqual(new Envelope().CorrelationId);
            new Envelope().CorrelationId.ShouldNotEqual(new Envelope().CorrelationId);
            new Envelope().CorrelationId.ShouldNotEqual(new Envelope().CorrelationId);
            new Envelope().CorrelationId.ShouldNotEqual(new Envelope().CorrelationId);
            new Envelope().CorrelationId.ShouldNotEqual(new Envelope().CorrelationId);
        }

        [Test]
        public void does_not_override_an_existing_correlation_id()
        {
            var headers = new NameValueHeaders();
            headers[Envelope.IdKey] = "FOO";

            var envelope = new Envelope(headers);
            envelope.CorrelationId.ShouldEqual("FOO");
        }

        [Test]
        public void will_assign_a_new_correlation_id_if_none_in_headers()
        {
            new Envelope(new NameValueHeaders()).CorrelationId
                .IsEmpty().ShouldBeFalse();
        }

        [Test]
        public void default_values_for_original_and_parent_id_are_null()
        {
            var parent = new Envelope
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            parent.OriginalId.ShouldBeNull();
            parent.ParentId.ShouldBeNull();
        }

        [Test]
        public void original_message_creating_child_envelope()
        {
            var parent = new Envelope
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            var childMessage = new Message1();

            var child = parent.ForResponse(childMessage);

            child.Message.ShouldBeTheSameAs(childMessage);

            child.OriginalId.ShouldEqual(parent.CorrelationId);
            child.ParentId.ShouldEqual(parent.CorrelationId);
        }

        [Test]
        public void parent_that_is_not_original_creating_child_envelope()
        {
            var parent = new Envelope
            {
                CorrelationId = Guid.NewGuid().ToString(),
                OriginalId = Guid.NewGuid().ToString()
            };

            var childMessage = new Message1();

            var child = parent.ForResponse(childMessage);

            child.Message.ShouldBeTheSameAs(childMessage);

            child.OriginalId.ShouldEqual(parent.OriginalId);
            child.ParentId.ShouldEqual(parent.CorrelationId);
        }

        [Test]
        public void if_reply_requested_header_exists_in_parent_and_matches_the_message_type()
        {
            var parent = new Envelope
            {
                CorrelationId = Guid.NewGuid().ToString(),
                OriginalId = Guid.NewGuid().ToString(),
                ReplyUri = "foo://bar".ToUri(),
                ReplyRequested = typeof(Message1).Name
            };

            var childMessage = new Message1();

            var child = parent.ForResponse(childMessage);

            child.Headers[Envelope.ResponseIdKey].ShouldEqual(parent.CorrelationId);
            child.Destination.ShouldEqual(parent.ReplyUri);
        }


        [Test]
        public void if_reply_requested_header_exists_in_parent_and_does_NOT_match_the_message_type()
        {
            var parent = new Envelope
            {
                CorrelationId = Guid.NewGuid().ToString(),
                OriginalId = Guid.NewGuid().ToString(),
                ReplyUri = "foo://bar".ToUri(),
                ReplyRequested = typeof(Message2).Name
            };

            var childMessage = new Message1();

            var child = parent.ForResponse(childMessage);

            child.Headers.Has(Envelope.ResponseIdKey).ShouldBeFalse();
            child.Destination.ShouldBeNull();
        }

        [Test]
        public void do_not_set_destination_or_response_if_requested_header_does_not_exist_in_parent()
        {
            var parent = new Envelope
            {
                CorrelationId = Guid.NewGuid().ToString(),
                OriginalId = Guid.NewGuid().ToString(),
                Source = "foo://bar".ToUri()
            };

            parent.Headers.Has(Envelope.ReplyRequestedKey).ShouldBeFalse();

            var childMessage = new Message1();

            var child = parent.ForResponse(childMessage);

            child.Headers.Has(Envelope.ResponseIdKey).ShouldBeFalse();
            child.Destination.ShouldBeNull();
        }

        [Test]
        public void source_property()
        {
            var envelope = new Envelope();

            envelope.Source.ShouldBeNull();

            var uri = "fake://thing".ToUri();
            envelope.Source = uri;

            envelope.Headers[Envelope.SourceKey].ShouldEqual(uri.ToString());
            envelope.Source.ShouldEqual(uri);
        }

        [Test]
        public void reply_uri_property()
        {
            var envelope = new Envelope();

            envelope.ReplyUri.ShouldBeNull();

            var uri = "fake://thing".ToUri();
            envelope.ReplyUri = uri;

            envelope.Headers[Envelope.ReplyUriKey].ShouldEqual(uri.ToString());
            envelope.ReplyUri.ShouldEqual(uri);
        }


        [Test]
        public void content_type()
        {
            var envelope = new Envelope();
            envelope.ContentType.ShouldEqual(null);

            envelope.ContentType = "text/xml";

            envelope.Headers[Envelope.ContentTypeKey].ShouldEqual("text/xml");
            envelope.ContentType.ShouldEqual("text/xml");
        }

        [Test]
        public void original_id()
        {
            var envelope = new Envelope();
            envelope.OriginalId.ShouldBeNull();

            var originalId = Guid.NewGuid().ToString();
            envelope.OriginalId = originalId;

            envelope.Headers[Envelope.OriginalIdKey].ShouldEqual(originalId);
            envelope.OriginalId.ShouldEqual(originalId);
        }

        [Test]
        public void ParentId()
        {
            var envelope = new Envelope();
            envelope.ParentId.ShouldBeNull();

            var parentId = Guid.NewGuid().ToString();
            envelope.ParentId = parentId;

            envelope.Headers[Envelope.ParentIdKey].ShouldEqual(parentId);
            envelope.ParentId.ShouldEqual(parentId);
        }

        [Test]
        public void ResponseId()
        {
            var envelope = new Envelope();
            envelope.ResponseId.ShouldBeNull();

            var responseId = Guid.NewGuid().ToString();
            envelope.ResponseId = responseId;

            envelope.Headers[Envelope.ResponseIdKey].ShouldEqual(responseId);
            envelope.ResponseId.ShouldEqual(responseId);
        }

        [Test]
        public void destination_property()
        {
            var envelope = new Envelope();

            envelope.Destination.ShouldBeNull();

            var uri = "fake://thing".ToUri();
            envelope.Destination = uri;

            envelope.Headers[Envelope.DestinationKey].ShouldEqual(uri.ToString());
            envelope.Destination.ShouldEqual(uri);
        }

        [Test]
        public void received_at_property()
        {
            var envelope = new Envelope();

            envelope.ReceivedAt.ShouldBeNull();

            var uri = "fake://thing".ToUri();
            envelope.ReceivedAt = uri;

            envelope.Headers[Envelope.ReceivedAtKey].ShouldEqual(uri.ToString());
            envelope.ReceivedAt.ShouldEqual(uri);
        }

        [Test]
        public void reply_requested()
        {
            var envelope = new Envelope();
            envelope.ReplyRequested.ShouldBeNull();


            envelope.ReplyRequested = "Foo";
            envelope.Headers[Envelope.ReplyRequestedKey].ShouldEqual("Foo");
            envelope.ReplyRequested.ShouldEqual("Foo");

            envelope.ReplyRequested = null;
            envelope.ReplyRequested.ShouldBeNull();
        }

        [Test]
        public void ack_requested()
        {
            var envelope = new Envelope();
            envelope.AckRequested.ShouldBeFalse();


            envelope.AckRequested = true;
            envelope.Headers[Envelope.AckRequestedKey].ShouldEqual("true");
            envelope.AckRequested.ShouldBeTrue();

            envelope.AckRequested = false;
            envelope.Headers.Has(Envelope.AckRequestedKey).ShouldBeFalse();
        }

        [Test]
        public void execution_time_is_null_by_default()
        {
            new Envelope().ExecutionTime.ShouldBeNull();
        }

        [Test]
        public void execution_time_set_and_get()
        {
            var time = DateTime.Today.AddHours(8).ToUniversalTime();

            var envelope = new Envelope();
            envelope.ExecutionTime = time;

            envelope.ExecutionTime.ShouldEqual(time);
        }

        [Test]
        public void nulling_out_the_execution_time()
        {
            var time = DateTime.Today.AddHours(8).ToUniversalTime();

            var envelope = new Envelope();
            envelope.ExecutionTime = time;

            envelope.ExecutionTime = null;

            envelope.ExecutionTime.ShouldBeNull();
        }

        [Test]
        public void use_serializer()
        {
            var envelope = new Envelope
            {
                Data = new byte[] {1, 2, 3, 4}
            };

            var serializer = MockRepository.GenerateMock<IEnvelopeSerializer>();
            var theExpectedMessage = new object();
            serializer.Stub(x => x.Deserialize(envelope)).Return(theExpectedMessage);

            envelope.UseSerializer(serializer);

            envelope.Message.ShouldBeTheSameAs(theExpectedMessage);
        }

        [Test]
        public void to_token()
        {
            var envelope = new Envelope
            {
                Data = new byte[] {1, 3, 4, 4},
                Message = new Message1()
            };

            var token = envelope.ToToken();

            // Hey, this HAS to work, so we're UT'ing it.
            token.Headers.ShouldBeTheSameAs(envelope.Headers);
            token.Message.ShouldEqual(envelope.Message);
            token.Data.ShouldEqual(envelope.Data);
        }

        [Test]
        public void attempts()
        {
            var envelope = new Envelope();
            envelope.Attempts.ShouldEqual(0);

            envelope.Attempts++;

            envelope.Attempts.ShouldEqual(1);
        }

        [Test]
        public void cloning_an_envelope()
        {
            var envelope = new Envelope();
            envelope.Message = new Message1();
            envelope.Headers["a"] = "1";
            envelope.Headers["b"] = "2";

            var clone = envelope.Clone();

            clone.ShouldNotBeTheSameAs(envelope);
            clone.Message.ShouldBeTheSameAs(envelope.Message);
            clone.Headers.ShouldNotBeTheSameAs(envelope.Headers);

            clone.Headers["a"].ShouldEqual("1");
            clone.Headers["b"].ShouldEqual("2");
        }
    }

    
}