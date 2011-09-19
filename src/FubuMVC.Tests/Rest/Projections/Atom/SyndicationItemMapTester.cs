using System;
using System.ServiceModel.Syndication;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Projections.Atom;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Rest.Projections.Atom
{
    [TestFixture]
    public class SyndicationItemMapTester
    {
        [Test]
        public void set_the_title()
        {
            var subject = new ItemSubject(){
                Title = "first",
                Title2 = "second"
            };

            var target = new SimpleValues<ItemSubject>(subject);

            var map1 = new SyndicationItemMap<ItemSubject>();
            map1.Title(x => x.Title);

            var map2 = new SyndicationItemMap<ItemSubject>();
            map2.Title(x => x.Title2);

            var item1 = new SyndicationItem();
            map1.ConfigureItem(item1, target);
            item1.Title.Text.ShouldEqual(subject.Title);


            var item2 = new SyndicationItem();
            map2.ConfigureItem(item2, target);
            item2.Title.Text.ShouldEqual(subject.Title2);

        }

        [Test]
        public void set_the_title_item_title_is_null_so_ignore_it()
        {
            var subject = new ItemSubject()
            {
                Title = null
            };

            var target = new SimpleValues<ItemSubject>(subject);

            var map1 = new SyndicationItemMap<ItemSubject>();
            map1.Title(x => x.Title);

            var item1 = new SyndicationItem();
            map1.ConfigureItem(item1, target);
            item1.Title.ShouldBeNull();
        }

        [Test]
        public void set_the_id_1()
        {
            var subject = new ItemSubject{
                Id = "001"
            };

            var target = new SimpleValues<ItemSubject>(subject);

            var map = new SyndicationItemMap<ItemSubject>(x => x.Id(o => o.Id));

            var item = new SyndicationItem();
            map.ConfigureItem(item, target);

            item.Id.ShouldEqual("001");
        }

        [Test]
        public void set_the_id_2()
        {
            var subject = new ItemSubject
            {
                Number = 333
            };

            var target = new SimpleValues<ItemSubject>(subject);

            var map = new SyndicationItemMap<ItemSubject>(x => x.Id(o => o.Number));

            var item = new SyndicationItem();
            map.ConfigureItem(item, target);

            item.Id.ShouldEqual("333"); 
        }

        [Test]
        public void set_the_updated()
        {
            var subject = new ItemSubject
            {
                Number = 333,
                Updated = DateTime.Today.AddDays(-3)
            };

            var target = new SimpleValues<ItemSubject>(subject);

            var map = new SyndicationItemMap<ItemSubject>(x => x.UpdatedByProperty(o => o.Updated));

            var item = new SyndicationItem();
            map.ConfigureItem(item, target);

            item.LastUpdatedTime.Date.ShouldEqual(subject.Updated.Date);
        }
    }

    public class ItemSubject
    {
        public string Title { get; set; }
        public string Title2 { get; set; }

        public string Id { get; set; }
        public int Number { get; set; }

        public DateTime Updated { get; set; }
    }
}