using System;
using System.Web.UI;

namespace FubuMVC.WebForms
{
    public static class WebFormExtensions
    {
        /// <summary>
        /// Gets the relative path to a view markup file for the specified strongly typed view
        /// </summary>
        /// <param name="viewType">The type of the compiled view</param>
        /// <returns></returns>
        public static string ToVirtualPath(this Type viewType)
        {
            // TODO -- need to get some unit tests at this thing
            // TODO -- bogus.  Need a WebForm type to url strategy that's pluggable
            //string[] nameParts = viewType.FullName.Split('.');
            //string path = "~";
            //for (int i = 2; i < nameParts.Length; i++)
            //{
            //    string part = nameParts[i];
            //    path += "/" + part;
            //}
            string path = viewType.FullName.Replace(viewType.Assembly.GetName().Name, "").TrimStart('.');

            path = "~/" + path.Replace('.', '/');

            if (typeof (UserControl).IsAssignableFrom(viewType))
            {
                path += ".ascx";
            }
            else if (typeof (MasterPage).IsAssignableFrom(viewType))
            {
                path += ".master";
            }
            else
            {
                path += ".aspx";
            }

            return path;
        }
    }
}