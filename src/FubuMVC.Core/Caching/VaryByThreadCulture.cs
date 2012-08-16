using System.Collections.Generic;
using System.Threading;

namespace FubuMVC.Core.Caching
{
    public class VaryByThreadCulture : IVaryBy
    {
        public IDictionary<string, string> Values()
        {
            return new Dictionary<string, string> { {"culture", Thread.CurrentThread.CurrentUICulture.Name} };
        }
    }
}