using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using FubuTestApplication.Domain;
using StoryTeller;
using StoryTeller.Engine;
using System.Linq;

namespace IntegrationTesting.Fixtures.FubuFastPack
{
    public class QueryServiceFixture : Fixture
    {
        public QueryServiceFixture()
        {
            Title = "Out of the Box QueryService";

            this["MixedNumberAndStrings"] =
                verifyFor<QueriedGrid>("For a grid with both string and number columns the filter options should be");
        }

        public IGrammar StringFieldsShouldBeFilteredWith()
        {
            return
                VerifyStringList(() => operatorsFor(x => x.Title)).Titled("Operators for a string property should be").
                    Grammar();
        }

        public IGrammar NumberFieldsShouldBeFilteredWith()
        {
            return
                VerifyStringList(() => operatorsFor(x => x.Number)).Titled("Operators for a number property should be").
                    Grammar();
        }

        private IGrammar verifyFor<T>(string title) where T : ISmartGrid, new()
        {
            return VerifySetOf<QueryOption>(() =>
            {
                var grid = new T();
                var service = new QueryService(new IFilterHandler[0]);
                var allFilteredProperties = grid.AllFilteredProperties(service);
                return allFilteredProperties.SelectMany(x => x.Operators.Select(o => new QueryOption(){
                    Operator = o.Key,
                    PropertyName = x.Accessor.Name
                }));
            })
            .Titled(title).MatchOn(x => x.Operator, x => x.PropertyName);
        }


        private IEnumerable<string> operatorsFor(Expression<Func<Case, object>> property)
        {
            var service = new QueryService(new IFilterHandler[0]);
            return service.FilterOptionsFor<Case>(property.ToAccessor()).Select(x => x.Key);
        }   
     

        public class QueryOption
        {
            public string PropertyName { get; set; }
            public string Operator { get; set;}
        }
    }

    public class QueriedGrid : RepositoryGrid<Case>
    {
        public QueriedGrid()
        {
            Show(x => x.Title);
            Show(x => x.Identifier);
            Show(x => x.Number);
        }
    }
}