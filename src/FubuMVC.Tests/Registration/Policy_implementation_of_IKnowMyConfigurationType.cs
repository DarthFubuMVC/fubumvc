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

    }


}