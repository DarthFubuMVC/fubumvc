using System;
using FubuCore;

namespace FubuMVC.Core.ServiceBus
{
    public static class StringExtensions
    {
         public static Uri ToUri(this string uriString)
         {
             return uriString.IsEmpty() ? null : new Uri(uriString);
         }
    }
}