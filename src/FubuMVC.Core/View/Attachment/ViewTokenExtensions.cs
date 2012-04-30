using FubuCore;

namespace FubuMVC.Core.View.Attachment
{
    public static class ViewTokenExtensions
    {
        public static IViewToken Resolve(this IViewToken token)
        {
            if (token is ProfileViewToken)
            {
                return token.As<ProfileViewToken>().View;
            }

            return token;
        }
    }
}