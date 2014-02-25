using FubuMVC.Core;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class Policy_implementation_of_IKnowMyConfigurationType
    {
        private Policy thePolicy;

        [SetUp]
        public void SetUp()
        {
            thePolicy = new Policy();
        }

        private string theConfigurationType()
        {
            return thePolicy.As<IKnowMyConfigurationType>().DetermineConfigurationType();
        }

        [Test]
        public void with_a_single_lambda_modification()
        {
            thePolicy.ModifyBy(chain => { }, configurationType:ConfigurationType.InjectNodes);

            theConfigurationType().ShouldEqual(ConfigurationType.InjectNodes);
        }


        [Test]
        public void if_multiple_actions_use_the_one_latest_in_the_cycle()
        {
            thePolicy.Conneg.AcceptJson();
            thePolicy.ModifyBy(chain => { }, configurationType: ConfigurationType.Attachment);

            // Attachement is after Conneg
            theConfigurationType().ShouldEqual(ConfigurationType.Attachment);
        }


        [Test]
        public void attribute_on_the_policy_wins()
        {
            new CustomPolicy().As<IKnowMyConfigurationType>().DetermineConfigurationType()
                              .ShouldEqual(ConfigurationType.InjectNodes);
        }
    }

    [ConfigurationType(ConfigurationType.InjectNodes)]
    public class CustomPolicy : Policy
    {
        public CustomPolicy()
        {
            Conneg.AcceptJson();
        }
    }
}