using System;
using System.Web.Razor;

namespace FubuMVC.Razor.Core
{
    public class RazorCodeLanguageFactory
    {
         public static RazorCodeLanguage Create(string extension)
         {
             RazorCodeLanguage language;
             switch (extension)
             {
                 case ".cshtml":
                     language = new FubuCSharpRazorCodeLanguage();
                     break;
                 case ".vbhtml":
                     language = new VBRazorCodeLanguage();
                     break;
                 default:
                     throw new ArgumentException("Invalid extension for Razor engine.");
             }
             return language;
         }
    }
}