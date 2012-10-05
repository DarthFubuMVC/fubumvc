using System.Collections.Generic;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class TransformationPolicyComparerTester
    {
        private TransformerComparer theComparer;
        private TransformerPolicy generate;
        private TransformerPolicy substitution;
        private TransformerPolicy _transformer;
        private TransformerPolicy batched;
        private TransformerPolicy global;
        private JavascriptTransformerPolicy<StubTransformer> generateA;
        private JavascriptTransformerPolicy<StubTransformer> generateB;
        private JavascriptTransformerPolicy<StubTransformer> generateC;
        private JavascriptTransformerPolicy<StubTransformer> generate2;
        private JavascriptTransformerPolicy<StubTransformer> generate3;

        [SetUp]
        public void SetUp()
        {
            var file = new AssetFile("something.a.b.c.d.js");
            theComparer = new TransformerComparer(file);

            generate = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Generate);
            generate2 = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Generate);
            generate3 = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Generate);
            generateA = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Generate, ".a");
            generateB = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Generate, ".b");
            generateC = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Generate, ".c");
            substitution = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Substitution);
            _transformer = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Transformation);
            batched = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.BatchedTransformation);
            global = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Global);
        }

        private IList<ITransformerPolicy> sort(params ITransformerPolicy[] policies)
        {
            var list = new List<ITransformerPolicy>(policies);
            list.Sort(theComparer);

            return list;
        }

        [Test]
        public void compare_by_actions()
        {
            sort(generate, substitution).ShouldHaveTheSameElementsAs(generate, substitution);
            sort(substitution, generate).ShouldHaveTheSameElementsAs(generate, substitution);
           
            sort(substitution, global).ShouldHaveTheSameElementsAs(substitution, global);
            sort(global, substitution).ShouldHaveTheSameElementsAs(substitution, global);

            sort(batched, global, generate, substitution, _transformer)
                .ShouldHaveTheSameElementsAs(generate, substitution, _transformer, batched, global);

            sort(batched, global, generate, substitution, _transformer)
                .ShouldHaveTheSameElementsAs(generate, substitution, _transformer, batched, global);

        
        }

        [Test]
        public void order_by_extension_position_within_the_same_family_of_actions_when_all_extensions_can_be_found()
        {
            // The file name is something.a.b.c.d.js
            sort(generateB, generateA, generateC).ShouldHaveTheSameElementsAs(generateA, generateB, generateC);
            sort(generateA, generateB, generateC).ShouldHaveTheSameElementsAs(generateA, generateB, generateC);
            sort(generateC, generateA, generateB).ShouldHaveTheSameElementsAs(generateA, generateB, generateC);
            
            sort(generateC, generateA).ShouldHaveTheSameElementsAs(generateA, generateC);
            sort(generateC, generateB).ShouldHaveTheSameElementsAs(generateB, generateC);
        }

        [Test]
		[Platform(Exclude="Mono")]
        public void order_with_must_be_after_rules_if_extension_and_action_do_not_catch()
        {
            generate2.AddMustBeAfterRule(p => p == generate);
            generate3.AddMustBeAfterRule(p => p == generate2);
            
            generate3.AddMustBeAfterRule(p => p == generateA);

            sort(generate2, generate, generate3).ShouldHaveTheSameElementsAs(generate, generate2, generate3);
            sort(generate, generate3, generate2).ShouldHaveTheSameElementsAs(generate, generate2, generate3);
            sort(generate, generate2, generate3).ShouldHaveTheSameElementsAs(generate, generate2, generate3);
            sort(generate3, generate2, generate).ShouldHaveTheSameElementsAs(generate, generate2, generate3);
        
            sort(generate3, generateA).ShouldHaveTheSameElementsAs(generateA, generate3);
            sort(generateA, generate3).ShouldHaveTheSameElementsAs(generateA, generate3);


        
        }

        [Test]
        public void return_zero_for_ordering_if_there_is_no_possible_way_to_sort()
        {
            var generate4 = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Generate, ".4");

            var comparer = new TransformerComparer(new AssetFile("something.js"));
            comparer.Compare(generate, generate4).ShouldEqual(0);
            comparer.Compare(generate4, generate).ShouldEqual(0);
        }
    }
}