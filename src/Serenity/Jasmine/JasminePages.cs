using System;
using System.Collections.Generic;
using FubuMVC.Core;
using HtmlTags;
using FubuMVC.StructureMap;
using StructureMap;

namespace Serenity.Jasmine
{
    public class JasminePages
    {
        public HtmlDocument Home()
        {
            return new HtmlDocument(){
                Title = "Serenity Jasmine Tester"
            };
        }
    }

    public class SerenityJasmineRegistry : FubuRegistry
    {
        public SerenityJasmineRegistry()
        {
            Actions.IncludeType<JasminePages>();
            Routes.HomeIs<JasminePages>(x => x.Home());
        }
    }

    public class SerenityJasmineApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            // TODO -- need to add more stuff here
            return FubuApplication
                .For<SerenityJasmineRegistry>()
                .StructureMap(new Container());
        }

        public string Name
        {
            get { return "Serenity Jasmine Runner"; }
        }
    }

    public class SerenityAppHost
    {
        public void AddDirectory(string folder)
        {
            throw new NotImplementedException();
        }


    }
}