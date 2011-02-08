using FubuMVC.Core.Security;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    public static class AuthorizationOnPageExtensions
    {
        public static T ReadOnlyIfNotAuthorized<T>(this T tag) where T : HtmlTag
        {
            if (!tag.Authorized() && tag.TagName() == "a")
            {
                tag.Authorized(true);
                tag.TagName("span");
            }

            return tag;
        }

        public static HtmlTag RequiresAccessTo(this HtmlTag tag, params string[] roleName)
        {
            return tag.Authorized(tag.Authorized() && PrincipalRoles.IsInRole(roleName));
        }



        public static HtmlTag ReadOnly(this HtmlTag tag)
        {
            return ReadOnly(tag, true);
        }

        public static HtmlTag ReadOnly(this HtmlTag tag, bool condition)
        {
            if (condition)
            {
                tag.Attr("disabled", "disabled");
            }
            return tag;
        }
    }
}