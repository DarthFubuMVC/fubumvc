using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Combination
{
    [TestFixture]
    public class AssetTagPlanTester
    {
        private AssetFileDataMother theFiles;

        [SetUp]
        public void SetUp()
        {
            theFiles = new AssetFileDataMother((path, file) => { });
            theFiles.LoadAssets(@"
a=scripts/a.js
b=scripts/b.js
c=scripts/c.js
d=scripts/d.js
e=scripts/e.js
f=scripts/f.js
g=scripts/g.js
h=scripts/h.js
i=scripts/i.js
j=scripts/j.js
k=scripts/k.js
");
        }



        private ScriptFileCombination combinationFor(string text)
        {
            var names = text.Split(',');
            var files = names.Select(x => theFiles[x]);

            return new ScriptFileCombination(files);
        }

        private AssetTagPlan planFor(string text)
        {
            var names = text.Split(',');
            var files = names.Select(x => theFiles[x]);

            return new AssetTagPlan(MimeType.Javascript, files);
        }

        [Test]
        public void try_to_find_a_sequence_that_is_too_long_from_that_point_should_return_null()
        {
            var plan = planFor("a,b,c,d,e,f,g,h,i,j,k");
            plan.TryFindSequenceStartingWith(theFiles["a"], 20).ShouldBeNull();
            plan.TryFindSequenceStartingWith(theFiles["i"], 5).ShouldBeNull();

        }

        [Test]
        public void try_to_find_a_sequence_where_the_start_is_not_found()
        {
            var plan = planFor("a,b,c,d,e,f,g,h,i,j,k");
            plan.TryFindSequenceStartingWith(new AssetFile("something.js"), 2).ShouldBeNull();
        }

        [Test]
        public void try_to_find_a_sequence_success()
        {
            var plan = planFor("a,b,c,d,e,f,g,h,i,j,k");
            plan.TryFindSequenceStartingWith(theFiles["b"], 3).Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("b.js", "c.js", "d.js");

            plan.TryFindSequenceStartingWith(theFiles["b"], 2).Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("b.js", "c.js");

            plan.TryFindSequenceStartingWith(theFiles["b"], 5).Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("b.js", "c.js", "d.js", "e.js", "f.js");


        }

        [Test]
        public void try_finding_a_sequence_should_skip_if_there_are_any_combos_in_the_list()
        {
            var plan = planFor("a,b,c,d,e,f,g,h,i,j,k");
            var combo = combinationFor("c,d,e");
            plan.TryCombination(combo);

            plan.TryFindSequenceStartingWith(theFiles["b"], 3).ShouldBeNull();
        }


        [Test]
        public void replace_with_multiple_combinations_1()
        {
            var plan = planFor("a,b,c,d,e,f,g,h,i,j,k");

            var combo1 = combinationFor("b,c,d");
            var combo2 = combinationFor("f,g");
            var combo3 = combinationFor("i,j,k");

            plan.TryCombination(combo1);
            plan.TryCombination(combo2);
            plan.TryCombination(combo3);

            plan.Subjects.Select(x => x.Name).ShouldHaveTheSameElementsAs("a.js", combo1.Name, "e.js", combo2.Name, "h.js", combo3.Name);
        }

        [Test]
        public void replace_with_multiple_combinations_2()
        {
            var plan = planFor("a,b,c,d,e,f,g,h,i,j,k");

            var combo1 = combinationFor("a,b,c,d");
            var combo2 = combinationFor("e,f,g");
            var combo3 = combinationFor("i,j");

            plan.TryCombination(combo1);
            plan.TryCombination(combo2);
            plan.TryCombination(combo3);

            plan.Subjects.Select(x => x.Name).ShouldHaveTheSameElementsAs(combo1.Name, combo2.Name, "h.js", combo3.Name, "k.js");
        }

        [Test]
        public void negative_match_on_combination_because_the_files_mismatch()
        {
            var plan = planFor("a,b,c");
            var combo = combinationFor("d,e,f");

            plan.TryCombination(combo).ShouldBeFalse();
        }

        [Test]
        public void negative_match_on_combination_because_the_order_is_wrong()
        {
            var plan = planFor("a,b,c,d,e");
            var combo = combinationFor("e,d,c");

            plan.TryCombination(combo).ShouldBeFalse();
        }

        [Test]
        public void negative_match_on_combination_not_all_the_files_are_there_1()
        {
            var plan = planFor("a,b,c,d,e");
            var combo = combinationFor("d,e,f");

            plan.TryCombination(combo).ShouldBeFalse();
        }


        [Test]
        public void negative_match_on_combination_not_all_the_files_are_there_2()
        {
            var plan = planFor("b,c,d,e");
            var combo = combinationFor("a,b,c");

            plan.TryCombination(combo).ShouldBeFalse();
        }

        [Test]
        public void positive_match_with_exact_order()
        {
            var plan = planFor("a,b,c,d,e");
            var combo = combinationFor("b,c,d");

            plan.TryCombination(combo).ShouldBeTrue();
        }

        [Test]
        public void positive_match_replaces_the_individual_files_with_the_combination_in_the_right_place_1()
        {
            var plan = planFor("a,b,c,d,e");
            var combo = combinationFor("b,c,d");

            plan.TryCombination(combo);

            plan.Subjects.Select(x => x.Name).ShouldHaveTheSameElementsAs("a.js", combo.Name, "e.js");
        }

        [Test]
        public void positive_match_replaces_the_individual_files_with_the_combination_in_the_right_place_2()
        {
            var plan = planFor("a,b,c,d,e,f");
            var combo = combinationFor("a,b,c");

            plan.TryCombination(combo);

            plan.Subjects.Select(x => x.Name).ShouldHaveTheSameElementsAs(combo.Name, "d.js", "e.js", "f.js");
        }


        [Test]
        public void positive_match_replaces_the_individual_files_with_the_combination_in_the_right_place_3()
        {
            var plan = planFor("a,b,c,d,e,f");
            var combo = combinationFor("e,f");

            plan.TryCombination(combo);

            plan.Subjects.Select(x => x.Name).ShouldHaveTheSameElementsAs("a.js", "b.js", "c.js", "d.js", combo.Name);
        }


        [Test]
        public void positive_match_replaces_the_individual_files_with_the_combination_in_the_right_place_4()
        {
            var plan = planFor("a,b,c,d,e,f");
            var combo = combinationFor("b,c");

            plan.TryCombination(combo);

            plan.Subjects.Select(x => x.Name).ShouldHaveTheSameElementsAs("a.js", combo.Name, "d.js", "e.js", "f.js");
        }

        [Test]
        public void positive_match_replaces_the_individual_files_with_the_combination_in_the_right_place_5()
        {
            var plan = planFor("a,b,c,d,e,f");
            var combo = combinationFor("b,c,d,e");

            plan.TryCombination(combo);

            plan.Subjects.Select(x => x.Name).ShouldHaveTheSameElementsAs("a.js", combo.Name, "f.js");
        }


        [Test]
        public void remove_missing()
        {
            var plan = planFor("a,b,c,d");
            var plan2 = planFor("e,f,g");
            var plan3 = planFor("h,i");
            plan.AddSubjects(new IAssetTagSubject[]{new MissingAssetTagSubject("s1.js"), new MissingAssetTagSubject("s2.js")});
        
            plan.AddSubjects(plan2.Subjects);

            plan.AddSubjects(new IAssetTagSubject[] { new MissingAssetTagSubject("s3.js"), new MissingAssetTagSubject("s4.js") });

            plan.AddSubjects(plan3.Subjects);

            var missing = plan.RemoveMissingAssets();
            missing.Select(x => x.Name).ShouldHaveTheSameElementsAs("s1.js", "s2.js", "s3.js", "s4.js");

            plan.Subjects.Select(x => x.Name).ShouldHaveTheSameElementsAs("a.js", "b.js", "c.js", "d.js", "e.js", "f.js", "g.js", "h.js", "i.js");
        }

    }
}