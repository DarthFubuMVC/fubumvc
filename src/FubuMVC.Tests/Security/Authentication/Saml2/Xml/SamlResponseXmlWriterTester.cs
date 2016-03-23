using System.Diagnostics;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Xml 
{
    [TestFixture]
    public class SamlResponseXmlWriterTester
    {
        private SamlResponse theOriginalResponse;
        private XmlDocument document;
        private SamlResponse theSecondResponse;

        // TODO -- try to verify this against the real spec XSD

        [TestFixtureSetUp]
        public void SetUp()
        {
            var xml = new FileSystem().ReadStringFromFile("sample.xml");
            theOriginalResponse = new SamlResponseXmlReader(xml).Read();

            document = new SamlResponseXmlWriter(theOriginalResponse).Write();
            
            Debug.WriteLine(document.OuterXml);
            
            theSecondResponse = new SamlResponseXmlReader(document.OuterXml).Read();
        }

        [Test]
        public void writes_the_assertion_element()
        {
            var assertion = document.DocumentElement.FindChild("Assertion");
            assertion.ShouldNotBeNull();
            assertion.GetAttribute("ID").ShouldBe("SamlAssertion-A5092bc640a235880200023f80002aa33");
            assertion.GetAttribute("IssueInstant").ShouldBe("2012-11-01T18:16:04Z");

            var issuer = assertion.FirstChild;
            issuer.Name.ShouldBe("saml2:Issuer");
            issuer.InnerText.ShouldBe("urn:idp:fakecompany:nbpartgenoutbds20:uat");
        }

        [Test]
        public void version_should_be_2_point_0()
        {
            document.DocumentElement.GetAttribute("Version").ShouldBe("2.0");
        }

        [Test]
        public void writes_the_issuer()
        {
            theSecondResponse.Issuer.ShouldBe(theOriginalResponse.Issuer);
        }

        [Test]
        public void writes_the_status()
        {
            theSecondResponse.Status.ShouldBe(theOriginalResponse.Status);
        }

        [Test]
        public void writes_the_Destination()
        {
            theSecondResponse.Destination.ShouldBe(theOriginalResponse.Destination);
        }

        [Test]
        public void writes_the_Id()
        {
            theSecondResponse.Id.ShouldBe("SamlResponse-"+theOriginalResponse.Id);
        }

        [Test]
        public void writes_the_issue_instant()
        {
            theSecondResponse.IssueInstant.ShouldBe(theOriginalResponse.IssueInstant);
        }

        [Test]
        public void writes_the_subject_name()
        {
            theSecondResponse.Subject.Name.ShouldBe(theOriginalResponse.Subject.Name);
        }

        [Test]
        public void writes_the_subject_format()
        {
            theSecondResponse.Subject.Name.Format
                             .ShouldBe(theOriginalResponse.Subject.Name.Format);
        }

        [Test]
        public void writes_the_subject_confirmation_methods()
        {
            theSecondResponse.Subject.Confirmations.Select(x => x.Method)
                .ShouldHaveTheSameElementsAs(theOriginalResponse.Subject.Confirmations.Select(x => x.Method));

        }
        
        [Test]
        public void writes_the_subject_confirmation_name()
        {
            theSecondResponse.Subject.Confirmations.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs(theOriginalResponse.Subject.Confirmations.Select(x => x.Name));

        }

        [Test]
        public void writes_the_subject_confirmation_data()
        {
            var secondConfirmationData = theSecondResponse.Subject.Confirmations.First().ConfirmationData.First();
            var originalConfirmationData = theOriginalResponse.Subject.Confirmations.First().ConfirmationData.First();

            secondConfirmationData.NotOnOrAfter.ShouldBe(originalConfirmationData.NotOnOrAfter);
            secondConfirmationData.Recipient.ShouldBe(originalConfirmationData.Recipient);
        }

        [Test]
        public void writes_the_condition_group_times()
        {
            theSecondResponse.Conditions.NotBefore
                             .ShouldBe(theOriginalResponse.Conditions.NotBefore);

            theSecondResponse.Conditions.NotOnOrAfter
                             .ShouldBe(theOriginalResponse.Conditions.NotOnOrAfter);
        }

        [Test]
        public void writes_the_audiences()
        {
            var secondAudiences = theSecondResponse.Conditions.Conditions.OfType<AudienceRestriction>().Select(x => x.Audiences);
            var originalAudiences =
                theOriginalResponse.Conditions.Conditions.OfType<AudienceRestriction>().Select(x => x.Audiences);


            secondAudiences.ShouldHaveTheSameElementsAs(originalAudiences);
        }

        [Test]
        public void writes_the_authentication_context_basic_properties()
        {
            theSecondResponse.Authentication.Instant.ShouldBe(theOriginalResponse.Authentication.Instant);
            theSecondResponse.Authentication.SessionIndex.ShouldBe(theOriginalResponse.Authentication.SessionIndex);
            theSecondResponse.Authentication.SessionNotOnOrAfter.ShouldBe(theOriginalResponse.Authentication.SessionNotOnOrAfter);
        }

        [Test]
        public void writes_the_authentication_context_declaration_reference()
        {
            theSecondResponse.Authentication.DeclarationReference
                             .ShouldBe(theOriginalResponse.Authentication.DeclarationReference);
        }
        
        [Test]
        public void writes_the_authentication_context_class_reference()
        {
            theSecondResponse.Authentication.ClassReference
                             .ShouldBe(theOriginalResponse.Authentication.ClassReference);
        }

        [Test]
        public void writes_the_attributes()
        {
            theSecondResponse.Attributes.Get("ClientId")
                             .ShouldBe("000012345");

            theSecondResponse.Attributes.Get("CustomerId")
                             .ShouldBe("001010111");
        }
    }
}