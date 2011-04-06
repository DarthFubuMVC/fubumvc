using System.Collections.Generic;
using FubuFastPack.Querying;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuFastPack.Testing.Querying
{
    [TestFixture]
    public class QueryableDataSourceFilterTester
    {
        private QueryableDataSourceFilter<Case> filter;
        private IQueryable<Case> theCases;

        [SetUp]
        public void SetUp()
        {
            theCases = new List<Case>(){
                new Case(){Title = "this one", Condition = "open"},
                new Case(){Title = "that one", Condition = "closed"},
                new Case(){Title = "that other one", Condition = "open"},
                new Case(){Title = "this other one", Condition = "closed"},
                new Case(){Title = "different one", Condition = "different"},
            }.AsQueryable();

            filter = new QueryableDataSourceFilter<Case>();
        }


        [Test]
        public void where_equal_testing()
        {
            filter.WhereEqual(x => x.Condition, "open");
            filter.Filter(theCases).Select(x => x.Title)
                .ShouldHaveTheSameElementsAs("this one", "that other one");
        }


        [Test]
        public void where_not_equal_testing()
        {
            filter.WhereNotEqual(x => x.Condition, "closed");
            filter.Filter(theCases).Select(x => x.Title)
                .ShouldHaveTheSameElementsAs("this one", "that other one", "different one");
        }
    }
}