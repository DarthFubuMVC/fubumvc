using System;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Transformation;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Transformations
{
    [TestFixture]
    public class TransformationPolicyTester
    {
        [Test]
        public void matching_extension_position()
        {
            var policy = new TransformationPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof (StubTransformer));

            policy.MatchingExtensionPosition(new[]{".js", ".coffee"}).HasValue.ShouldBeFalse();
            policy.AddExtension(".coffee");
            policy.AddExtension(".else");
            policy.MatchingExtensionPosition(new[] { ".js", ".coffee" }).ShouldEqual(1);
        }

        [Test]
        public void transformation_policy_should_blow_up_if_the_type_is_not_concrete_of_IAssetTransformer()
        {
            // Wrong concrete type
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new TransformationPolicy(ActionType.Generate, MimeType.Javascript, GetType());
            });

            // Not a concrete type
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new TransformationPolicy(ActionType.Generate, MimeType.Javascript, typeof(ExtendingIAssetTransformer));
            });
        }

        [Test]
        public void applies_to_negative_on_all()
        {
            var policy = new TransformationPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));
 
            policy.AddExtension(".coffee");

            var file = new AssetFile("script.js");

            policy.AppliesTo(file).ShouldBeFalse();
        }

        [Test]
        public void applies_to_positive_based_on_mimetype()
        {
            var policy = new TransformationPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));

            policy.AddExtension(".coffee");
            policy.AddExtension(".cf");

            policy.AppliesTo(new AssetFile("script.coffee")).ShouldBeTrue();
            policy.AppliesTo(new AssetFile("script.coffee.js")).ShouldBeTrue();
            policy.AppliesTo(new AssetFile("script.cf")).ShouldBeTrue();
            policy.AppliesTo(new AssetFile("script.cf.js")).ShouldBeTrue();
            policy.AppliesTo(new AssetFile("script.something.cf.js")).ShouldBeTrue();
        }

        [Test]
        public void applies_to_positive_and_negative_based_on_other_criteria()
        {
            var policy = new TransformationPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));
  
            policy.AddMatchingCriteria(file => file.Name.StartsWith("yes"));

            policy.AppliesTo(new AssetFile("yes.js")).ShouldBeTrue();
            policy.AppliesTo(new AssetFile("no.js")).ShouldBeFalse();
        }

        [Test]
        public void must_be_after_is_false_by_default()
        {
            var policy = new TransformationPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));

            var policy2 = new TransformationPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));

            policy.MustBeAfter(policy2).ShouldBeFalse();
            policy2.MustBeAfter(policy).ShouldBeFalse();
        }
    }

    public interface ExtendingIAssetTransformer : IAssetTransformer
    {
        
    }

    public class StubTransformer : IAssetTransformer
    {
        public string Transform(IContentSource inner)
        {
            throw new NotImplementedException();
        }
    }

    public class StubTransformer2 : IAssetTransformer
    {
        public string Transform(IContentSource inner)
        {
            throw new NotImplementedException();
        }
    }
}