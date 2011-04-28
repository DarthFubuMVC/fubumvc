//using System.Linq;
//using FubuMVC.Spark.SparkModel;
//using FubuTestingSupport;
//using NUnit.Framework;
//using Rhino.Mocks;
//using Spark.Compiler;

//namespace FubuMVC.Spark.Tests.SparkModel
//{
//    public class SparkItemBuilderTester : InteractionContext<SparkItemBuilder>
//    {
//        private ISparkItemPolicy _policy1;
//        private ISparkItemPolicy _policy2;

//        private ISparkItemBinder _binder1;
//        private ISparkItemBinder _binder2;

//        private SparkItem _item1;
//        private SparkItem _item2;

//        protected override void beforeEach()
//        {
//            _item1 = new SparkItem("item1.spark", "x", "o1");
//            _item2 = new SparkItem("item2.spark", "z", "o2");

//            _policy1 = MockFor<ISparkItemPolicy>();
//            _policy2 = MockFor<ISparkItemPolicy>();

//            _binder1 = MockFor<ISparkItemBinder>();
//            _binder2 = MockFor<ISparkItemBinder>();

//            var chunkLoader = MockFor<IChunkLoader>();
//            chunkLoader.Stub(x => x.Load(Arg<SparkItem>.Is.Anything)).Return(Enumerable.Empty<Chunk>());

//            Services.InjectArray(new[] { _item1, _item2, });
//            Services.Inject(chunkLoader);

//            configurePolicies();
//            configureBinders();

//            registerBindersAndPolicies();

//        }

//        private void registerBindersAndPolicies()
//        {
//            ClassUnderTest.AddBinder(_binder1);
//            ClassUnderTest.AddBinder(_binder2);

//            ClassUnderTest.Apply(_policy1);
//            ClassUnderTest.Apply(_policy2);
//        }

//        private void configurePolicies()
//        {
//            _policy1.Stub(x => x.Matches(_item1)).Return(true);
//            _policy1.Stub(x => x.Matches(Arg<SparkItem>.Is.NotEqual(_item1))).Return(false);
//            _policy2.Stub(x => x.Matches(Arg.Is(_item2))).Return(true);
//            _policy2.Stub(x => x.Matches(Arg<SparkItem>.Is.NotEqual(_item2))).Return(false);
//        }

//        private void configureBinders()
//        {
//            _binder1.Expect(x => x.Bind(Arg.Is(_item1), Arg<BindContext>.Is.Anything));
//            _binder1.Expect(x => x.Bind(Arg.Is(_item2), Arg<BindContext>.Is.Anything));
//            _binder2.Expect(x => x.Bind(Arg.Is(_item1), Arg<BindContext>.Is.Anything));
//            _binder2.Expect(x => x.Bind(Arg.Is(_item2), Arg<BindContext>.Is.Anything));
//        }


//        [Test]
//        public void binders_are_applied_against_each_spark_item()
//        {
//            ClassUnderTest.BuildItems();
//            _binder1.VerifyAllExpectations(); // this also verifies expectations on _binder2
//        }

//        [Test]
//        public void policies_that_match_are_applied_against_a_spark_item()
//        {
//            _policy1.Expect(x => x.Apply(_item1)).Repeat.Once();
//            _policy2.Expect(x => x.Apply(_item2)).Repeat.Once();
//            ClassUnderTest.BuildItems();
//            _policy1.VerifyAllExpectations();
//        }

//        [Test]
//        public void policies_that_do_not_match_are_not_applied_against_a_spark_item()
//        {
//            _policy1.Expect(x => x.Apply(_item2)).Repeat.Never();
//            _policy2.Expect(x => x.Apply(_item1)).Repeat.Never();
//            ClassUnderTest.BuildItems();
//            _policy1.VerifyAllExpectations();
//        }

//    }
//}