using FubuMVC.Core.Security;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    public static class HtmlTagExtensions
    {
        
    }

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

        public static HtmlTag RequiresAccessTo(this HtmlTag tag, string roleName)
        {
            return tag.Authorized(tag.Authorized() && PrincipalRoles.IsInRole(roleName));
        }
    }


}