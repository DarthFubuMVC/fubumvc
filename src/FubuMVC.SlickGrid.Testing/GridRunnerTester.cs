using System;
using System.Collections.Generic;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.SlickGrid.Testing
{
    [TestFixture]
    public class GridRunnerTester
    {
        [Test]
        public void run_with_no_query()
        {
            var runner = new GridRunner<Foo, FooGrid, FooSource>(new FooGrid(), new FooSource());
            var dicts = runner.Run();

            dicts.Select(x => x["name"]).ShouldHaveTheSameElementsAs("Scooby", "Shaggy", "Velma");
        }

        [Test]
        public void run_with_query()
        {
            var runner = new GridRunner<Foo, FooGrid, FancyFooSource, FooQuery>(new FooGrid(), new FancyFooSource());
            var dicts = runner.Run(new FooQuery{Letter = "S"});

            dicts.Select(x => x["name"]).ShouldHaveTheSameElementsAs("Scooby", "Shaggy");
        }
    }

    public class FooQuery
    {
        public string Letter { get; set; }
    }

    public class FancyFooSource : IGridDataSource<Foo, FooQuery>
    {
        public IEnumerable<Foo> GetData(FooQuery query)
        {
            return new FooSource().GetData().Where(x => x.Name.StartsWith(query.Letter));
        }
    }

    public class FooSource : IGridDataSource<Foo>
    {
        public IEnumerable<Foo> GetData()
        {
            yield return new Foo{
                Name = "Scooby"
            };

            yield return new Foo
            {
                Name = "Shaggy"
            };

            yield return new Foo(){
                Name = "Velma"
            };
        }
    }

    public class FooGrid : IGridDefinition<Foo>
    {
        public IEnumerable<IDictionary<string, object>> FormatData(IEnumerable<Foo> data)
        {
            return data.Select(x =>
            {
                return new Dictionary<string, object>{
                    {"name", x.Name}
                };
            });
        }

        public string ToColumnJson()
        {
            throw new NotImplementedException();
        }

        public string SelectDataSourceUrl(IUrlRegistry urls)
        {
            throw new NotImplementedException();
        }
    }

    public class Foo
    {
        public string Name { get; set; }
    }
}