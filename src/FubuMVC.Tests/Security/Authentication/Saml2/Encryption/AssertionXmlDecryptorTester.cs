using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using FubuMVC.Core.Security.Authentication.Saml2.Encryption;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Encryption
{
    [TestFixture]
    public class AssertionXmlDecryptorTester
    {
        [Test]
        public void triple_des()
        {
            AssertionXmlDecryptor.GetSymmetricBlockEncryptionAlgorithm(EncryptedXml.XmlEncTripleDESUrl)
                                 .ShouldBeOfType<TripleDESCryptoServiceProvider>();
        }

        [Test]
        public void des_crypto()
        {
            AssertionXmlDecryptor.GetSymmetricBlockEncryptionAlgorithm(EncryptedXml.XmlEncDESUrl)
                                 .ShouldBeOfType<DESCryptoServiceProvider>();
        }

        [Test]
        public void rijndael_128()
        {
            AssertionXmlDecryptor.GetSymmetricBlockEncryptionAlgorithm(EncryptedXml.XmlEncAES128Url)
                                 .ShouldBeOfType<RijndaelManaged>()
                                 .KeySize.ShouldBe(128);
        }

        [Test]
        public void rijndael_192()
        {
            AssertionXmlDecryptor.GetSymmetricBlockEncryptionAlgorithm(EncryptedXml.XmlEncAES192Url)
                                 .ShouldBeOfType<RijndaelManaged>()
                                 .KeySize.ShouldBe(192);
        }

        [Test]
        public void rijndael_256()
        {
            AssertionXmlDecryptor.GetSymmetricBlockEncryptionAlgorithm(EncryptedXml.XmlEncAES256Url)
                                 .ShouldBeOfType<RijndaelManaged>()
                                 .KeySize.ShouldBe(256);
        }

        [Test]
        public void unrecognized_encryption()
        {
            Exception<Exception>.ShouldBeThrownBy(() => {
                AssertionXmlDecryptor.GetSymmetricBlockEncryptionAlgorithm("Nothing valid");
            }).Message.ShouldContain("Unrecognized symmetric encryption algorithm URI 'Nothing valid'");
        }
    }
}