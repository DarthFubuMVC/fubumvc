namespace FubuMVC.Core.Security.Authentication.Saml2.Certificates
{
    public interface ICertificate
    {
        string Issuer { get; }
        string SerialNumber { get; }
        bool IsVerified { get; }
    }
}