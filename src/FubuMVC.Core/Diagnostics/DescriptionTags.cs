using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics
{
    public class DescriptionBodyTag : HtmlTag
    {
        public DescriptionBodyTag(Description description)
            : base("div")
        {
            AddClass("description-body");

            addDescriptionText(description);

            writeProperties(description);

            writeChildren(description);

            description.BulletLists.Each(writeBulletList);
        }

        private void writeBulletList(BulletList list)
        {
            Add("div").AddClass("desc-list-name").Text(list.Label ?? list.Name);
            list.Children.Each(writeBulletItem);
        }

        private void writeBulletItem(Description bullet)
        {
            Add("div").AddClass("desc-bullet-item-title").Text(bullet.Title);
            Add("div").AddClass("desc-bullet-item-body").Append(new DescriptionBodyTag(bullet));
        }

        private void writeChildren(Description description)
        {
            description.Children.Each((name, child) => Append(new ChildDescriptionTag(name, child)));
        }

        private void writeProperties(Description description)
        {
            if (description.Properties.Any())
            {
                var table = new TableTag();
                table.AddClass("desc-properties");

                description.Properties.Each((key, value) =>
                {
                    table.AddBodyRow(row =>
                    {
                        row.Header(key);
                        row.Cell(value);
                    });
                });

                Append(table);
            }


        }

        private void addDescriptionText(Description description)
        {
            if (description.HasExplicitShortDescription())
            {
                Add("p").AddClass("short-desc").Text(description.ShortDescription);
            }

            if (description.LongDescription.IsNotEmpty())
            {
                Add("p").AddClass("long-desc").Text(description.LongDescription);
            }
        }
    }

    public class DescriptionPropertyTag : HtmlTag
    {
        public DescriptionPropertyTag(string key, string value)
            : base("div")
        {
            AddClass("desc-prop");
            Add("b").Text(key);
            Add("span").Text(value);
        }
    }

    public class ChildDescriptionTag : HtmlTag
    {
        public ChildDescriptionTag(string name, Description child)
            : base("div")
        {
            AddClass("desc-child");

            Add("div", title =>
            {
                title.AddClass("desc-child-title");
                title.Add("b").Text(name);
                title.Add("i").Text(child.Title);
            });

            Add("div").AddClass("desc-child-body").Append(new DescriptionBodyTag(child));
        }
    }

}