using System;
using System.Collections.Generic;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.AntiForgery.Testing
{
    [TestFixture]
    public class AntiForgeryTokenProviderTester : InteractionContext<AntiForgeryTokenProvider>
    {
        [Test]
        public void names_from_path_are_asp_compatible()
        {
            const string aspName = "__RequestVerificationToken_Pz4-Pj8.Pz4-Pj8.Pz4-Pg__";
            //Token name for path:
            const string original = "?>?>?>?>?>?>?>?>";
            string name = ClassUnderTest.GetTokenName(original);

            name.ShouldEqual(aspName);
        }


        [Test]
        public void names_without_path_are_asp_compatible()
        {
            const string aspName = "__RequestVerificationToken";
            //Token name without path

            string name = ClassUnderTest.GetTokenName();
            name.ShouldEqual(aspName);
        }

        [Test]
        public void provided_tokens_are_correct_length()
        {
            AntiForgeryData token = ClassUnderTest.GenerateToken();

            byte[] decodedValue = Convert.FromBase64String(token.Value);
            decodedValue.Length.ShouldEqual(16);
        }

        [Test]
        public void provided_tokens_are_random()
        {
            var tokens = new List<string>();

            Enumerable.Range(0, 1000000).Each(x => tokens.Add(ClassUnderTest.GenerateToken().Value));

            tokens.Distinct().Count().ShouldEqual(tokens.Count);
        }
    }
}