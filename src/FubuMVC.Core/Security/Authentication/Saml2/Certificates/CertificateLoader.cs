using System;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using FubuCore;

namespace FubuMVC.Core.Security.Authentication.Saml2.Certificates
{
    public class CertificateLoader : ICertificateLoader
    {
        public X509Certificate2 Load(string thumbprint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

            try
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                // TODO -- should the FindType be editable?
                var certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

                if (certs.Count == 0)
                {
                    throw new UnknownCertificateException(thumbprint);
                }

                return certs[0];
            }
            finally
            {
                store.Close();
            }
        }
    }

    [Serializable]
    public class UnknownCertificateException : Exception
    {
        public UnknownCertificateException(string thumbprint) : base("Unable to load certificate {0} by thumbprint".ToFormat(thumbprint))
        {
        }

        protected UnknownCertificateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}