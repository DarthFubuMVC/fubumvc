using System;
using System.Collections.Generic;
using Fubu.Templating;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Templating
{
    public class KeywordReplacerTester : InteractionContext<KeywordReplacer>
    {

        [Test]
        public void SetToken_Should_Add_Token_To_KeywordReplacer()
        {
            var token = "TESTTOKEN";
            ClassUnderTest.SetToken(token, "");
            ClassUnderTest.GetToken(token).ShouldNotBeNull();
        }

        [Test]
        public void SetTokens_Should_Allow_For_Adding_Multiple_Keywords()
        {
            var tokens = new Dictionary<string, string>
                         {
                             {"TOKEN0", "REPLACEMENT0"},
                             {"TOKEN1", "REPLACEMENT1"},
                             {"TOKEN2", "REPLACEMENT2"}
                         };

            ClassUnderTest.SetTokens(tokens);

            for (int i = 0; i < 3; i++)
            {
                ClassUnderTest.GetToken("TOKEN" + i).ShouldEqual("REPLACEMENT" + i);
            }
        }

        [Test]
        public void GetToken_Should_Return_Replacement_For_Token()
        {
            var token = "FOOTOKEN";
            var replacement = "FOOREPLACE";

            ClassUnderTest.SetToken(token, replacement);
            ClassUnderTest.GetToken(token).ShouldEqual(replacement);
        }

        [Test]
        public void ContainsToken_Should_Return_True_When_KeywordReplacer_Contains_Token()
        {
            var token = "MYTOKEN";
            ClassUnderTest.SetToken(token, "");
            ClassUnderTest.ContainsToken(token).ShouldBeTrue();
        }

        [Test]
        public void ContainsToken_Should_Return_False_When_KeywordReplacer_Doesnt_Contain_Token()
        {
            var token = "MYTOKEN";
            ClassUnderTest.SetToken(token, "");
            ClassUnderTest.ContainsToken("NOTOKEN").ShouldBeFalse();
        }

        [Test]
        public void Replace_Should_Replace_Text_With_Token_Replacement_Values()
        {
            var tokens = new Dictionary<string, string> 
                         {
                            {"FUBUTOKEN", "FUBUREPLACEMENT" },
                            {"FUBUTESTTOKEN", "FUBUTESTTOKENREPLACED"}
                         };

            var replacementText = "This is a FUBUTOKEN test of the FUBUTOKEN ReplaceMethod. FUBUTESTTOKEN should be replaced.";
            var expectedText = "This is a FUBUREPLACEMENT test of the FUBUREPLACEMENT ReplaceMethod. FUBUTESTTOKENREPLACED should be replaced.";

            ClassUnderTest.SetTokens(tokens);
            var actualText = ClassUnderTest.Replace(replacementText);
            actualText.ShouldEqual(expectedText);
        }

        [Test]
        public void should_cache_guid_replacements()
        {
            var replacementText = "GUID1,GUID1,GUID1,GUID2";
            var actualText = ClassUnderTest.Replace(replacementText);

            var values = actualText.Split(new[] {','}, StringSplitOptions.None);
            var first = values[0];
            Guid.Parse(first);
            values[1].ShouldEqual(first);
            values[2].ShouldEqual(first);
            values[3].ShouldNotEqual(first);
        }
    }
}