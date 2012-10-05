using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class ContentPlannerIntegratedTester
    {
        [Test]
        public void build_a_plan_for_a_single_file_that_does_not_match_any_transforms()
        {
            ContentPlanScenario.For(x =>
            {
                x.SingleAssetFileName = "script1.js";
                x.JsTransformer<CoffeeTransformer>(ActionType.Transformation, ".coffee");
            })
                .ShouldMatch(@"
FileRead:script1.js
");
        }

        [Test]
        public void build_a_plan_for_a_single_file_that_has_a_single_transform_that_is_not_batched()
        {
            ContentPlanScenario.For(x =>
            {
                x.SingleAssetFileName = "script1.coffee.js";
                x.JsTransformer<CoffeeTransformer>(ActionType.Transformation, ".coffee");
            })
                .ShouldMatch(@"
Transform:CoffeeTransformer
  FileRead:script1.coffee.js
");
        }

        [Test]
        public void build_a_plan_for_a_single_file_that_has_multiple_transforms()
        {
            ContentPlanScenario.For(x =>
            {
                x.SingleAssetFileName = "script1.a.coffee.js";
                x.JsTransformer<CoffeeTransformer>(ActionType.Transformation, ".coffee");
                x.JsTransformer<ATransformer>(ActionType.Transformation, ".a");
            })
                .ShouldMatch(
                    @"
Transform:CoffeeTransformer
  Transform:ATransformer
    FileRead:script1.a.coffee.js
");
        }

        [Test]
        public void build_a_plan_for_a_single_file_with_no_transformers_of_any_kind()
        {
            ContentPlanScenario.For(x => { x.SingleAssetFileName = "script1.js"; })
                .ShouldMatch(@"
FileRead:script1.js
");
        }


        [Test]
        public void more_complex_batch_transform_scenario()
        {
            ContentPlanScenario.For(x =>
            {
                x.JsTransformer<CoffeeTransformer>(ActionType.BatchedTransformation, ".coffee");
                x.CombinationOfScriptsIs("my-scripts", "script1.coffee.js", "script2.coffee.js", "script3.coffee.js",
                                         "script4.js", "script5.js");
            })
                .ShouldMatch(
                    @"
Combination
  Transform:CoffeeTransformer
    Combination
      FileRead:script1.coffee.js
      FileRead:script2.coffee.js
      FileRead:script3.coffee.js
  FileRead:script4.js
  FileRead:script5.js
");
        }

        [Test]
        public void more_complex_batch_transform_scenario_2()
        {
            ContentPlanScenario.For(x =>
            {
                x.JsTransformer<CoffeeTransformer>(ActionType.BatchedTransformation, ".coffee");
                x.CombinationOfScriptsIs("my-scripts", "script4.js", "script5.js", "script1.coffee.js",
                                         "script2.coffee.js", "script3.coffee.js");
            })
                .ShouldMatch(
                    @"
Combination
  FileRead:script4.js
  FileRead:script5.js
  Transform:CoffeeTransformer
    Combination
      FileRead:script1.coffee.js
      FileRead:script2.coffee.js
      FileRead:script3.coffee.js
");
        }

        [Test]
        public void more_complex_batch_transform_scenario_3()
        {
            ContentPlanScenario.For(x =>
            {
                x.JsTransformer<CoffeeTransformer>(ActionType.BatchedTransformation, ".coffee");
                x.CombinationOfScriptsIs("my-scripts", "a.js", "b.js", "script1.coffee.js", "script2.coffee.js",
                                         "script3.coffee.js", "script4.js", "script5.js");
            })
                .ShouldMatch(
                    @"
Combination
  FileRead:a.js
  FileRead:b.js
  Transform:CoffeeTransformer
    Combination
      FileRead:script1.coffee.js
      FileRead:script2.coffee.js
      FileRead:script3.coffee.js
  FileRead:script4.js
  FileRead:script5.js
");
        }

        [Test]
        public void more_complex_batch_transform_scenario_4()
        {
            ContentPlanScenario.For(x =>
            {
                x.JsTransformer<CoffeeTransformer>(ActionType.BatchedTransformation, ".coffee");
                x.CombinationOfScriptsIs("my-scripts", "a.js", "b.js", "script1.coffee.js", "script2.coffee.js",
                                         "script3.coffee.js", "script4.js", "script5.js", "script6.coffee.js");
            })
                .ShouldMatch(
                    @"
Combination
  FileRead:a.js
  FileRead:b.js
  Transform:CoffeeTransformer
    Combination
      FileRead:script1.coffee.js
      FileRead:script2.coffee.js
      FileRead:script3.coffee.js
  FileRead:script4.js
  FileRead:script5.js
  Transform:CoffeeTransformer
    FileRead:script6.coffee.js
");
        }

        [Test]
        public void multiple_files_should_be_combined()
        {
            ContentPlanScenario.For(
                x => { x.CombinationOfScriptsIs("my-scripts", "script1.js", "script2.js", "script3.js"); })
                .ShouldMatch(@"
Combination
  FileRead:script1.js
  FileRead:script2.js
  FileRead:script3.js
");
        }

        [Test]
        public void should_only_pick_up_transforms_for_the_right_mimetype()
        {
            ContentPlanScenario.For(x =>
            {
                x.SingleAssetFileName = "script1.a.coffee.js";
                x.JsTransformer<CoffeeTransformer>(ActionType.Transformation, ".coffee");
                x.JsTransformer<ATransformer>(ActionType.Transformation, ".a");
                x.CssTransformer<AnotherTransformer>(ActionType.Transformation, ".a");
            })
                .ShouldMatch(
                    @"
Transform:CoffeeTransformer
  Transform:ATransformer
    FileRead:script1.a.coffee.js
");
        }

        [Test]
        public void simple_batch_transform_scenario()
        {
            ContentPlanScenario.For(x =>
            {
                x.JsTransformer<CoffeeTransformer>(ActionType.BatchedTransformation, ".coffee");
                x.CombinationOfScriptsIs("my-scripts", "script1.coffee.js", "script2.coffee.js", "script3.coffee.js");
            })
                .ShouldMatch(
                    @"
Transform:CoffeeTransformer
  Combination
    FileRead:script1.coffee.js
    FileRead:script2.coffee.js
    FileRead:script3.coffee.js
");
        }


        [Test]
        public void apply_a_non_batched_global_transform_to_a_single_file()
        {
            ContentPlanScenario.For(x =>
            {
                x.TransformerPolicy<NotBatchedGlobalJsTransformer>();
                x.TransformerPolicy<NotBatchedGlobalCssTransformer>();

                x.SingleAssetFileName = "script1.js";
            })
            .ShouldMatch(@"
Transform:BTransformer
  FileRead:script1.js
");
        }

        [Test]
        public void apply_a_non_batched_global_transform_to_a_combo()
        {
            ContentPlanScenario.For(x =>
            {
                x.TransformerPolicy<NotBatchedGlobalJsTransformer>();
                x.TransformerPolicy<NotBatchedGlobalCssTransformer>();

                x.CombinationOfScriptsIs("combo1", "script1.js", "script2.js", "script3.js");
            })
            .ShouldMatch(@"
Combination
  Transform:BTransformer
    FileRead:script1.js
  Transform:BTransformer
    FileRead:script2.js
  Transform:BTransformer
    FileRead:script3.js
");
        }

        [Test]
        public void apply_a_batched_global_transform_to_a_combo()
        {
            ContentPlanScenario.For(x =>
            {
                x.TransformerPolicy<BatchedGlobalJsTransformer>();
                x.TransformerPolicy<BatchedGlobalCssTransformer>();

                x.CombinationOfScriptsIs("combo1", "script1.js", "script2.js", "script3.js");
            })
            .ShouldMatch(@"
Transform:ATransformer
  Combination
    FileRead:script1.js
    FileRead:script2.js
    FileRead:script3.js
");
        }

        [Test]
        public void apply_a_mix_of_tranformations_and_combos_batched_global_transform_to_a_combo()
        {
            ContentPlanScenario.For(x =>
            {
                x.TransformerPolicy<BatchedGlobalJsTransformer>();
                x.TransformerPolicy<BatchedGlobalCssTransformer>();

                x.JsTransformer<StubTransformer>(ActionType.Transformation, ".stub");

                x.CombinationOfScriptsIs("combo1", "script1.js", "script2.stub.js", "script3.js");
            })
            .ShouldMatch(@"
Transform:ATransformer
  Combination
    FileRead:script1.js
    Transform:StubTransformer
      FileRead:script2.stub.js
    FileRead:script3.js
");
        }


        [Test]
        public void apply_a_mix_of_tranformations_and_combos_batched_global_transform_to_a_combo_2()
        {
            ContentPlanScenario.For(x =>
            {
                x.TransformerPolicy<BatchedGlobalJsTransformer>();
                x.TransformerPolicy<BatchedGlobalCssTransformer>();

                x.JsTransformer<StubTransformer>(ActionType.Transformation, ".stub");
                x.JsTransformer<CoffeeTransformer>(ActionType.BatchedTransformation, ".coffee");



                x.CombinationOfScriptsIs("combo1", "script1.coffee.js", "script2.stub.coffee.js", "script3.js", "script4.coffee.js", "script5.stub.js");
            })
            .ShouldMatch(@"
Transform:ATransformer
  Combination
    Transform:CoffeeTransformer
      Combination
        FileRead:script1.coffee.js
        Transform:StubTransformer
          FileRead:script2.stub.coffee.js
    FileRead:script3.js
    Transform:CoffeeTransformer
      FileRead:script4.coffee.js
    Transform:StubTransformer
      FileRead:script5.stub.js
");
        }





    }

    public class BatchedGlobalJsTransformer : GlobalTransformerPolicy<ATransformer>
    {
        public BatchedGlobalJsTransformer() : base(MimeType.Javascript, BatchBehavior.MustBeBatched)
        {
        }
    }

    public class NotBatchedGlobalJsTransformer : GlobalTransformerPolicy<BTransformer>
    {
        public NotBatchedGlobalJsTransformer()
            : base(MimeType.Javascript, BatchBehavior.NoBatching)
        {
        }
    }

    public class BatchedGlobalCssTransformer : GlobalTransformerPolicy<CTransformer>
    {
        public BatchedGlobalCssTransformer()
            : base(MimeType.Css, BatchBehavior.MustBeBatched)
        {
        }
    }

    public class NotBatchedGlobalCssTransformer : GlobalTransformerPolicy<DTransformer>
    {
        public NotBatchedGlobalCssTransformer()
            : base(MimeType.Css, BatchBehavior.NoBatching)
        {
        }
    }


    public class ATransformer : ITransformer
    {
        public string Transform(string contents, IEnumerable<AssetFile> files)
        {
            throw new NotImplementedException();
        }
    }

    public class BTransformer : ITransformer
    {
        public string Transform(string contents, IEnumerable<AssetFile> files)
        {
            throw new NotImplementedException();
        }
    }

    public class CTransformer : ITransformer
    {
        public string Transform(string contents, IEnumerable<AssetFile> files)
        {
            throw new NotImplementedException();
        }
    }

    public class DTransformer : ITransformer
    {
        public string Transform(string contents, IEnumerable<AssetFile> files)
        {
            throw new NotImplementedException();
        }
    }

    public class AnotherTransformer : ITransformer
    {
        public string Transform(string contents, IEnumerable<AssetFile> files)
        {
            throw new NotImplementedException();
        }
    }

    public class CoffeeTransformer : ITransformer
    {
        public string Transform(string contents, IEnumerable<AssetFile> files)
        {
            throw new NotImplementedException();
        }
    }
}