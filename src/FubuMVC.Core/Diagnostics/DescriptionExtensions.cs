using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Diagnostics
{
    public static class DescriptionExtensions
    {
        public static IDictionary<string, object> ToDescriptiveDictionary(this object target)
        {
            return Description.For(target).ToDictionary();
        } 

        public static Dictionary<string, object> ToDictionary(this Description description)
        {
            var dictionary = new Dictionary<string, object>
            {
                {"type", "description"},
                {"title", description.Title},
                {"targetType", description.TargetType.FullName}
            };

            if (description.HasExplicitShortDescription()) dictionary.Add("description", description.ShortDescription);

            if (description.Properties.Count > 0)
            {
                dictionary.Add("properties", description.Properties.ToDictionary());
            }

            if (description.Children.Count > 0)
            {
                var children = new Dictionary<string, object>();
                dictionary.Add("children", children);

                description.Children.Each((key, desc) => children.Add(key, desc.ToDictionary()));
            }

            if (description.BulletLists.Count > 0)
            {
                var lists = new Dictionary<string, object>();
                dictionary.Add("lists", lists);

                description.BulletLists.Each(list =>
                {
                    lists.Add(list.Name, list.Children.Select(x => x.ToDictionary()).ToArray());
                });
            }


            return dictionary;
        } 
    }


}