using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DictionaryOutputConvention : Policy
    {
        public DictionaryOutputConvention()
        {
            Where.ResourceTypeImplements<IDictionary<string, object>>();
            Conneg.MakeAsymmetricJson();
        }
    }
}