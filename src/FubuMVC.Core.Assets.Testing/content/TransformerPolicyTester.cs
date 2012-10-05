using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class TransformerPolicyTester
    {
        [Test]
        public void matching_extension_position()
        {
            var policy = new TransformerPolicy(ActionType.Transformation, MimeType.Javascript,
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
                new TransformerPolicy(ActionType.Generate, MimeType.Javascript, GetType());
            });

            // Not a concrete type
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new TransformerPolicy(ActionType.Generate, MimeType.Javascript, typeof(IExtendingITransformer));
            });
        }

        [Test]
        public void applies_to_negative_on_all()
        {
            var policy = new TransformerPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));
 
            policy.AddExtension(".coffee");

            var file = new AssetFile("script.js");

            policy.AppliesTo(file).ShouldBeFalse();
        }

        [Test]
        public void applies_to_positive_based_on_mimetype()
        {
            var policy = new TransformerPolicy(ActionType.Transformation, MimeType.Javascript,
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
        public void applies_to_negative_based_on_exclusion_criteria()
        {
            var policy = new TransformerPolicy(ActionType.Transformation, MimeType.Javascript, typeof(StubTransformer));

            policy.AddExtension(".js");
            policy.AddExtension(".coffee");
            policy.AddExclusionCriteria(f => f.Name.Contains(".min."));

            policy.AppliesTo(new AssetFile("a.min.coffee")).ShouldBeFalse();
            policy.AppliesTo(new AssetFile("b.coffee")).ShouldBeTrue();
            policy.AppliesTo(new AssetFile("c.min.js")).ShouldBeFalse();
            policy.AppliesTo(new AssetFile("d.a.js")).ShouldBeTrue();
        }

        [Test]
        public void applies_to_positive_and_negative_based_on_other_criteria()
        {
            var policy = new TransformerPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));
  
            policy.AddMatchingCriteria(file => file.Name.StartsWith("yes"));
            policy.AddExclusionCriteria(f => f.Name.Contains(".man."));

            policy.AppliesTo(new AssetFile("yes.js")).ShouldBeTrue();
            policy.AppliesTo(new AssetFile("yes.man.js")).ShouldBeFalse();
            policy.AppliesTo(new AssetFile("no.js")).ShouldBeFalse();
        }

        [Test]
        public void must_be_after_is_false_by_default()
        {
            var policy = new TransformerPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));

            var policy2 = new TransformerPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));

            policy.MustBeAfter(policy2).ShouldBeFalse();
            policy2.MustBeAfter(policy).ShouldBeFalse();
        }

        [Test]
        public void use_must_be_after_rules()
        {
            var policy = new TransformerPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));

            var policy2 = new TransformerPolicy(ActionType.Transformation, MimeType.Javascript,
                                                  typeof(StubTransformer));

            policy2.AddMustBeAfterRule(p => p == policy);

            policy.MustBeAfter(policy2).ShouldBeFalse();
            policy2.MustBeAfter(policy).ShouldBeTrue();
        }

        [Test]
        public void must_be_batched()
        {
            new JavascriptTransformerPolicy<StubTransformer>(ActionType.Generate).MustBeBatched().ShouldBeFalse();
            new JavascriptTransformerPolicy<StubTransformer>(ActionType.Substitution).MustBeBatched().ShouldBeFalse();
            new JavascriptTransformerPolicy<StubTransformer>(ActionType.Transformation).MustBeBatched().ShouldBeFalse();
            new JavascriptTransformerPolicy<StubTransformer>(ActionType.BatchedTransformation).MustBeBatched().ShouldBeTrue();
            
        }

        [Test]
        public void global_action_in_transformer_policy_is_not_batched_by_default()
        {
            new JavascriptTransformerPolicy<StubTransformer>(ActionType.Global).MustBeBatched().ShouldBeFalse();
        }

        [Test]
        public void set_must_be_batched_in_global_transformer()
        {
            new GlobalTransformerPolicy<StubTransformer>(MimeType.Javascript, BatchBehavior.NoBatching)
                .MustBeBatched().ShouldBeFalse();

            new GlobalTransformerPolicy<StubTransformer>(MimeType.Javascript, BatchBehavior.MustBeBatched)
                .MustBeBatched().ShouldBeTrue();

            
        }
    }

    public interface IExtendingITransformer : ITransformer
    {
        
    }

    public class StubTransformer : ITransformer
    {
        public string Transform(string contents, IEnumerable<AssetFile> files)
        {
            throw new NotImplementedException();
        }
    }

    public class StubTransformer2 : ITransformer
    {
        public string Transform(string contents, IEnumerable<AssetFile> files)
        {
            throw new NotImplementedException();
        }
    }
}