﻿using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Projections;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Projections
{
    
    public class child_and_enumerable_children_sourced_by_services
    {
        private readonly Party theParty =
            new Party
            {
                Name = "Super boys",
                Land = "Two Rivers",
                Leader = "Rand Al'Thor"
            };

        private IDictionary<string, object> project(Projection<Party> projection)
        {
            var node = new DictionaryMediaNode();
            var services = new InMemoryServiceLocator();
            services.Add(new HeroRepository());

            var context = new ProjectionContext<Party>(services, theParty);

            projection.As<IProjection<Party>>().Write(context, node);

            return node.Values;
        }

        [Fact]
        public void inline_child()
        {
            var projection = new Projection<Party>();
            projection.Value(x => x.Name);
            projection
                .ForAttribute("Leader")
                .WriteChild(x => x.Service<HeroRepository>().FindHero(x.Subject.Leader))
                .Configure(x =>
                {
                    x.Value(o => o.FirstName);
                    x.Value(o => o.LastName);
                });

            var values = project(projection);

            var leader = values.Child("Leader");
            leader["FirstName"].ShouldBe("Rand");
            leader["LastName"].ShouldBe("Al'Thor");
        }

        [Fact]
        public void use_predefined_projection_for_the_child()
        {
            var projection = new Projection<Party>();
            projection.Value(x => x.Name);
            projection
                .ForAttribute("leader")
                .WriteChild(x => x.Service<HeroRepository>().FindHero(x.Subject.Leader))
                .Include<HeroProjection>();

            var values = project(projection);

            var leader = values.Child("leader");
            leader["first"].ShouldBe("Rand");
            leader["last"].ShouldBe("Al'Thor");
        }

        [Fact]
        public void use_inline_enumerable_found_by_service()
        {
            var projection = new Projection<Party>();
            projection.Value(x => x.Name);
            projection.ForAttribute("heros").WriteEnumerable(x => x.Service<HeroRepository>().For(x.Subject.Land))
                .DefineProjection(p =>
                {
                    p.Value(x => x.FirstName);
                    p.Value(x => x.LastName);
                });

            var values = project(projection);
            var heros = values.Children("heros");
            heros.Count().ShouldBe(3);
            heros.ElementAt(1)["FirstName"].ShouldBe("Mat");
            heros.ElementAt(2)["LastName"].ShouldBe("Aybara");
        }

        [Fact]
        public void use_pre_defined_projection_for_each_leaf_of_enumerable()
        {
            var projection = new Projection<Party>();
            projection.Value(x => x.Name);
            projection.ForAttribute("heros").WriteEnumerable(x => x.Service<HeroRepository>().For(x.Subject.Land))
                .UseProjection<HeroProjection>();

            var values = project(projection);
            var heros = values.Children("heros");
            heros.Count().ShouldBe(3);
            heros.ElementAt(1)["first"].ShouldBe("Mat");
            heros.ElementAt(2)["last"].ShouldBe("Aybara");
        }
    }

    public class HeroProjection : Projection<Hero>
    {
        public HeroProjection()
        {
            Value(x => x.FirstName).Name("first");
            Value(x => x.LastName).Name("last");
        }
    }

    public class Party
    {
        public string Name { get; set; }
        public string Leader { get; set; }
        public string Land { get; set; }
    }

    public class Hero
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Land
    {
        public string Name { get; set; }
    }

    public class HeroRepository
    {
        public IEnumerable<Hero> For(string land)
        {
            if (land == "Andor")
            {
                yield return new Hero { FirstName = "Elayne", LastName = "Trakand" };
            }

            if (land == "Two Rivers")
            {
                yield return new Hero { FirstName = "Rand", LastName = "Al'Thor" };
                yield return new Hero { FirstName = "Mat", LastName = "Cauthon" };
                yield return new Hero { FirstName = "Perrin", LastName = "Aybara" };
            }
        }

        public Hero FindHero(string name)
        {
            var names = name.Split(' ');
            return new Hero { FirstName = names.First(), LastName = names.Last() };
        }
    }
}