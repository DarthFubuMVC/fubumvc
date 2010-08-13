using System;
using System.Linq;

namespace FubuMVC.UI.Security
{
    public class AccessRight
    {
        private readonly bool _read;
        private readonly bool _write;
        private readonly int _permissiveness;

        private AccessRight(bool read, bool write, int permissiveness)
        {
            _read = read;
            _write = write;
            _permissiveness = permissiveness;
        }

        public int Permissiveness
        {
            get { return _permissiveness; }
        }

        public bool Read
        {
            get { return _read; }
        }

        public bool Write
        {
            get { return _write; }
        }

        public static readonly AccessRight All = new AccessRight(true, true, 1);
        public static readonly AccessRight ReadOnly = new AccessRight(true, false, 2);
        public static readonly AccessRight None = new AccessRight(false, false, 3);

        public static AccessRight operator +(AccessRight first, AccessRight second)
        {
            return first.Permissiveness <= second.Permissiveness ? first : second;
        }


        public static AccessRight Least(params AccessRight[] rights)
        {
            return rights.OrderByDescending(x => x.Permissiveness).FirstOrDefault();
        }

        public static AccessRight Most(params AccessRight[] rights)
        {
            return rights.OrderBy(x => x.Permissiveness).FirstOrDefault();
        }

        public override string ToString()
        {
            return string.Format("Read: {0}, Write: {1}", _read, _write);
        }
    }
}