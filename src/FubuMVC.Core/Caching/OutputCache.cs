using System;
using System.Collections.Generic;
using System.Threading;
using FubuCore.Util;
using FubuCore;

namespace FubuMVC.Core.Caching
{
    public class OutputCache : IOutputCache
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly Cache<string, IRecordedOutput> _outputs = new Cache<string, IRecordedOutput>();

        public IRecordedOutput Retrieve(string resourceHash, Func<IRecordedOutput> cacheMiss)
        {

            return _lock.MaybeWrite<IRecordedOutput>(
                answer:() => _outputs[resourceHash], 
                missingTest:() => !_outputs.Has(resourceHash),
                write:() => _outputs[resourceHash] = cacheMiss());
        }

        public void Eject(string resourceHash)
        {
            _lock.Write(() => _outputs.Remove(resourceHash));
        }

        public void FlushAll()
        {
            _lock.Write(() => _outputs.ClearAll());
        }

        public IEnumerable<string> AllCachedResources()
        {
            return _lock.Read(() => _outputs.GetAllKeys());
        }

    }
}