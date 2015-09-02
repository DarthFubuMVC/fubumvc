using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Runtime.Files
{
    public class ChangeSet
    {
        public readonly List<IFubuFile> Changed = new List<IFubuFile>();
        public readonly List<IFubuFile> Added = new List<IFubuFile>();
        public readonly List<string> Deleted = new List<string>();

        public ChangeSet Adds(params IFubuFile[] files)
        {
            Added.AddRange(files);
            return this;
        }

        public ChangeSet Changes(params IFubuFile[] files)
        {
            Changed.AddRange(files);
            return this;
        }

        public ChangeSet Deletes(params IFubuFile[] files)
        {
            Deleted.AddRange(files.Select(x => x.RelativePath));
            return this;
        }

        public bool HasChanges()
        {
            return Changed.Any() || Added.Any() || Deleted.Any();
        }

        public override string ToString()
        {
            return string.Format("changed: {0}, added: {1}, deleted: {2}", Changed.Select(x => x.RelativePath).Join(", "), Added.Select(x => x.RelativePath).Join(", "), Deleted.Join(", "));
        }
    }
}