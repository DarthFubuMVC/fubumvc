using System;

namespace FubuMVC.Core.Http.Compression
{
    // Just a marker to make the convention flow easier
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class DoNotCompressAttribute : Attribute
    {
    }
}