using System;
using FubuCore;

namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.NestedPartials
{
    public static class StringExtensions
    {
        public static string RemoveNewlines(this string value)
        {
            return value.IsEmpty() ? value : value.Replace(Environment.NewLine, "");
        }
    }
}