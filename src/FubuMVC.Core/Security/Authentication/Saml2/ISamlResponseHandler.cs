using System.Linq;
using FubuCore.Logging;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public interface ISamlResponseHandler
    {
        bool CanHandle(SamlResponse response);
        void Handle(ISamlDirector director, SamlResponse response);
    }

    public class SamlAuthenticationSucceeded : LogRecord
    {
        public SamlAuthenticationSucceeded()
        {
        }

        public SamlAuthenticationSucceeded(SamlResponse response)
        {
            Name = response.Subject.Name.Value;
            Issuer = response.Issuer;
            SamlId = response.Id;
        }

        public string Name { get; set; }

        public string Issuer { get; set; }

        public string SamlId { get; set; }
    }

    public class SamlAuthenticationFailed : LogRecord
    {
        public SamlAuthenticationFailed()
        {
        }

        public SamlAuthenticationFailed(SamlResponse response)
        {
            Errors = response.Errors.Select(x => x.Message).ToArray();
            Name = response.Subject.Name.Value;
            Issuer = response.Issuer;
            SamlId = response.Id;
        }

        public string Name { get; set; }
        public string[] Errors { get; set; }

        public string Issuer { get; set; }

        public string SamlId { get; set; }
    }
}