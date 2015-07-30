using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Http.Hosting;

namespace FubuMVC.Core
{
    public interface IApplication
    {
        
    }

    public class Application<T> where T : FubuRegistry, new()
    {
         public readonly T Registry;

        public Application(T registry)
        {
            Registry = registry;
        }

        public Application()
        {
            Registry = new T();
        }

        public string RootPath;
        //public int Port;
        //public IHost Host;
        //public string Mode;

        // Later, add this as a whilelist override
        //public Assembly[] PackageAssemblies { get; set; }

    }

    public class BasicApplication : Application<FubuRegistry>
    {
        public BasicApplication(FubuRegistry registry) : base(registry)
        {
        }

        public BasicApplication()
        {
        }


    }
}