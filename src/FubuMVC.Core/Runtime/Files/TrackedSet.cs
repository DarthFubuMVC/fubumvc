using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime.Files
{
    public class TrackedSet
    {
        private readonly Cache<string, DateTime> _files = new Cache<string, DateTime>();

        public TrackedSet(IEnumerable<IFubuFile> files)
        {
            files.Each(x => _files[x.RelativePath] = x.ExactLastWriteTime());
        }

        public IEnumerable<string> Files
        {
            get { return _files.GetAllKeys(); }
        }

        public ChangeSet DetectChanges(IEnumerable<IFubuFile> files)
        {
            var changeSet = new ChangeSet();

            var dict = files.ToDictionary(x => x.RelativePath);
            var deleted = _files.GetAllKeys().Where(x => !dict.ContainsKey(x));
            changeSet.Deleted.AddRange(deleted);

            
            changeSet.Added.AddRange(files.Where(x => !_files.Has(x.RelativePath)));


            var changed = files.Where(x => _files.Has(x.RelativePath) && _files[x.RelativePath] != x.ExactLastWriteTime());
            changeSet.Changed.AddRange(changed);

            return changeSet;

        }
    }
}