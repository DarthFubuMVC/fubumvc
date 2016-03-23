namespace FubuMVC.Core.Security.Authentication.Saml2
{


    public class SamlStatus : UriEnum<SamlStatus>
    {
        public static readonly SamlStatus Success = new SamlStatus("urn:oasis:names:tc:SAML:2.0:status:Success", "The request succeeded");
        public static readonly SamlStatus RequesterError = new SamlStatus("urn:oasis:names:tc:SAML:2.0:status:Requester", "The request could not be performed due to an error on the part of the requester.");
        public static readonly SamlStatus ResponderError = new SamlStatus("urn:oasis:names:tc:SAML:2.0:status:Responder", "The request could not be performed due to an error on the part of the SAML responder or SAML authority.");
        public static readonly SamlStatus VersionMismatch = new SamlStatus("urn:oasis:names:tc:SAML:2.0:status:VersionMismatch", "The SAML responder could not process the request because the version of the request message was incorrect.");

        private SamlStatus(string uri, string description) : base(uri, description)
        {
        }

        /*







The following second-level status codes are referenced at various places in this specification. Additional second-level status codes MAY be defined in future versions of the SAML specification. System entities are free to define more specific status codes by defining appropriate URI references.
urn:oasis:names:tc:SAML:2.0:status:AuthnFailed
The responding provider was unable to successfully authenticate the principal.
urn:oasis:names:tc:SAML:2.0:status:InvalidAttrNameOrValue Unexpected or invalid content was encountered within a <saml:Attribute> or
<saml:AttributeValue> element. urn:oasis:names:tc:SAML:2.0:status:InvalidNameIDPolicy
The responding provider cannot or will not support the requested name identifier policy.
urn:oasis:names:tc:SAML:2.0:status:NoAuthnContext
The specified authentication context requirements cannot be met by the responder.
urn:oasis:names:tc:SAML:2.0:status:NoAvailableIDP
Used by an intermediary to indicate that none of the supported identity provider <Loc> elements in an
<IDPList> can be resolved or that none of the supported identity providers are available. urn:oasis:names:tc:SAML:2.0:status:NoPassive
Indicates the responding provider cannot authenticate the principal passively, as has been requested.
urn:oasis:names:tc:SAML:2.0:status:NoSupportedIDP

urn:oasis:names:tc:SAML:2.0:status:PartialLogout
Used by a session authority to indicate to a session participant that it was not able to propagate logout to all other session participants.
urn:oasis:names:tc:SAML:2.0:status:ProxyCountExceeded
Indicates that a responding provider cannot authenticate the principal directly and is not permitted to proxy the request further.
urn:oasis:names:tc:SAML:2.0:status:RequestDenied
The SAML responder or SAML authority is able to process the request but has chosen not to respond. This status code MAY be used when there is concern about the security context of the request message or the sequence of request messages received from a particular requester.
urn:oasis:names:tc:SAML:2.0:status:RequestUnsupported
The SAML responder or SAML authority does not support the request.
urn:oasis:names:tc:SAML:2.0:status:RequestVersionDeprecated
The SAML responder cannot process any requests with the protocol version specified in the request.
urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooHigh
The SAML responder cannot process the request because the protocol version specified in the request message is a major upgrade from the highest protocol version supported by the responder.
urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooLow
The SAML responder cannot process the request because the protocol version specified in the request message is too low.
urn:oasis:names:tc:SAML:2.0:status:ResourceNotRecognized
The resource value provided in the request message is invalid or unrecognized.
urn:oasis:names:tc:SAML:2.0:status:TooManyResponses
The response message would contain more elements than the SAML responder is able to return.
urn:oasis:names:tc:SAML:2.0:status:UnknownAttrProfile
An entity that has no knowledge of a particular attribute profile has been presented with an attribute drawn from that profile.
urn:oasis:names:tc:SAML:2.0:status:UnknownPrincipal
The responding provider does not recognize the principal specified or implied by the request.
urn:oasis:names:tc:SAML:2.0:status:UnsupportedBinding
The SAML responder cannot properly fulfill the request using the protocol binding specified in the request.
         */

        public static SamlStatus Get(string text)
        {
            return get(text);
        }
    }
}