using System;
using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;

namespace SampleBottleService
{
    public class TestActivator :
        IActivator
    {
        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            Console.WriteLine("hi, poopy pants");


            packages.Select(p => p.Name)
                .Each(s => Console.WriteLine(s));
        }
    }
}