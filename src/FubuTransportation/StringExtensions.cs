using System;
using FubuCore;

namespace FubuTransportation
{
    public static class StringExtensions
    {
         public static Uri ToUri(this string uriString)
         {
             return uriString.IsEmpty() ? null : new Uri(uriString);
         }
    }
}