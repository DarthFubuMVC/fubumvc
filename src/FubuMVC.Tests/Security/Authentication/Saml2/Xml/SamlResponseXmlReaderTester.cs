using System;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Xml
{
    
    public class SamlResponseXmlReaderTester
    {
        private SamlResponseXmlReader theReader;
        private SamlResponse theResponse;

        public SamlResponseXmlReaderTester()
        {
            var xml = new FileSystem().ReadStringFromFile("sample.xml");
            theReader = new SamlResponseXmlReader(xml);

            theResponse = theReader.Read();
        }

        [Fact]
        public void reads_the_destination()
        {
            theResponse.Destination.ShouldBe("https://qa2.online.com/qa2/sso/saml".ToUri());
        }

        [Fact]
        public void reads_the_id()
        {
            theResponse.Id.ShouldBe("A5092bc640a235880200023f80002aa33");
        }

        [Fact]
        public void reads_the_issue_instant()
        {
            theResponse.IssueInstant.ShouldBe(XmlConvert.ToDateTimeOffset("2012-11-01T18:16:04Z"));
        }

        [Fact]
        public void read_the_issuer()
        {
            theResponse.Issuer.ShouldBe(new Uri("urn:idp:fakecompany:nbpartgenoutbds20:uat").ToString());
        }

        [Fact]
        public void read_the_status_of_the_response_if_it_is_success()
        {
            theResponse.Status.ShouldBe(SamlStatus.Success);
        }

        [Fact]
        public void can_read_the_condition_group_time_constraints()
        {
            theResponse.Conditions.NotBefore.ShouldBe(XmlConvert.ToDateTimeOffset("2012-11-01T18:13:04Z"));
            theResponse.Conditions.NotOnOrAfter.ShouldBe(XmlConvert.ToDateTimeOffset("2012-11-01T18:19:04Z"));
        }

        [Fact]
        public void can_read_audience_restriction()
        {
            var audienceRestricution = theResponse.Conditions.Conditions
                .Single()
                .ShouldBeOfType<AudienceRestriction>();

            audienceRestricution.Audiences.Single()
                                .ShouldBe(new Uri("https://qa2.online.com/qa2/sso/saml").ToString());
        }

        [Fact]
        public void can_read_the_subject_name()
        {
            theResponse.Subject.Name.ShouldBe(new SamlName
            {
                Type = SamlNameType.NameID,
                Value = "aa50045c6d0a233e7c20003d7d0000aa33",
                Format = NameFormat.Persistent
            });
        }

        [Fact]
        public void can_read_the_subject_confirmation_method()
        {
            theResponse.Subject.Confirmations.Single()
                       .Method.ShouldBe("urn:oasis:names:tc:SAML:2.0:cm:bearer".ToUri());
        }

        [Fact]
        public void subject_confirmation_data()
        {
            var data = theResponse.Subject.Confirmations.Single()
                                  .ConfirmationData.Single();

            data.NotOnOrAfter.ShouldBe(XmlConvert.ToDateTimeOffset("2012-11-01T18:19:04Z"));
            data.Recipient.ShouldBe("https://qa2.online.com/qa2/sso/saml".ToUri());

        }

        [Fact]
        public void has_the_attributes()
        {
            theResponse.Attributes.Get("ClientId").ShouldBe("000012345");
            theResponse.Attributes.Get("CustomerId").ShouldBe("001010111");
        }

        [Fact]
        public void can_read_the_certificates()
        {
            theResponse.Certificates.Any().ShouldBeTrue();
        }

        [Fact]
        public void reads_the_basic_authentication_properties()
        {
            theResponse.Authentication.Instant.ShouldBe(XmlConvert.ToDateTimeOffset("2012-11-01T18:15:47Z"));
            theResponse.Authentication.SessionIndex.ShouldBe("21171a497ffc16b87b8179afd5f9b69fc3f8cd8c");
            theResponse.Authentication.SessionNotOnOrAfter.ShouldBe(XmlConvert.ToDateTimeOffset("2012-11-02T04:15:47Z"));

        }

        [Fact]
        public void reads_the_authentication_context()
        {
            theResponse.Authentication.DeclarationReference
                       .ShouldBe("urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport".ToUri());
        }
        
        [Fact]
        public void reads_the_authentication_class_context()
        {
            theResponse.Authentication.ClassReference
                       .ShouldBe("urn:oasis:names:tc:SAML:2.0:ac:classes:Password".ToUri());
        }
    }
}