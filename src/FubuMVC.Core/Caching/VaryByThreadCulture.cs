using System;
using System.Collections.Generic;
using System.Threading;

namespace FubuMVC.Core.Caching
{
    public class VaryByThreadCulture : IVaryBy
    {
        public void Apply(IDictionary<string, string> dictionary)
        {
            dictionary.Add("culture", Thread.CurrentThread.CurrentUICulture.Name);
        }
    }
}